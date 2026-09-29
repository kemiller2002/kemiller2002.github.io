# Agent instructions

This repository participates in the Echelon engineering workflow.

## Working rules

- Preserve existing architecture and repository-specific conventions.
- Keep changes small, coherent, and recoverable.
- Commit and push incrementally at meaningful recovery boundaries.
- Do not wait for remote CI after every push. Continue the next independent in-scope slice while CI batches or runs.
- Run local checks when they inform implementation.
- Inspect remote build/CI status at the final implementation boundary by default.
- Inspect CI earlier only when its result gates the next action, protects a high-risk boundary, or is required for merge, release, or publication.
- Never treat queued, cancelled, unavailable, or unobserved CI as passing.
- Do not weaken validation, release, provenance, or security requirements merely to make CI faster.

## CI batching

Ordinary commit-driven validation should use a 10-minute quiet-period debounce and cancel superseded waiting work for the same ref. Explicit command/control workflows and irreversible release steps remain immediate unless they use a separate cancellable pre-side-effect debounce gate.
