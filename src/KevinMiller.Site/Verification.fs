namespace KevinMiller.Site

open System
open System.IO
open System.Security.Cryptography

type DeterminismResult =
    | Deterministic
    | Different of string list
    | BuildFailed of PublicationFinding list

module Verification =
    let private fileDigest path =
        use stream = File.OpenRead(path)
        use sha = SHA256.Create()
        sha.ComputeHash(stream)
        |> Convert.ToHexString

    let private snapshot root =
        Directory.EnumerateFiles(root, "*", SearchOption.AllDirectories)
        |> Seq.map (fun path ->
            let relative = Path.GetRelativePath(root, path).Replace('\\', '/')
            relative, fileDigest path)
        |> Map.ofSeq

    let private differences left right =
        let paths =
            Set.union
                (left |> Map.keys |> Set.ofSeq)
                (right |> Map.keys |> Set.ofSeq)

        paths
        |> Seq.choose (fun path ->
            match Map.tryFind path left, Map.tryFind path right with
            | Some a, Some b when a = b -> None
            | Some a, Some b -> Some $"{path}: {a} != {b}"
            | Some _, None -> Some $"{path}: missing from second build"
            | None, Some _ -> Some $"{path}: missing from first build"
            | None, None -> None)
        |> Seq.toList

    let verifyDeterministicBuild root =
        let tempRoot =
            Path.Combine(Path.GetTempPath(), "kevinmiller-site-determinism-" + Guid.NewGuid().ToString("N"))

        let first = Path.Combine(tempRoot, "first")
        let second = Path.Combine(tempRoot, "second")

        try
            match SiteBuild.build root first with
            | Error analysis -> BuildFailed analysis.Findings
            | Ok _ ->
                match SiteBuild.build root second with
                | Error analysis -> BuildFailed analysis.Findings
                | Ok _ ->
                    let delta = differences (snapshot first) (snapshot second)
                    if List.isEmpty delta then Deterministic else Different delta
        finally
            if Directory.Exists(tempRoot) then
                Directory.Delete(tempRoot, true)
