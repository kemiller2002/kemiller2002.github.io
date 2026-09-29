# Kevin Miller Site architecture records

The current accepted system architecture is summarized in
`context/ARCHITECTURE.md`.

Architecture records in this directory are for durable decisions that materially
change system boundaries, public contracts, core dependencies, security,
publication state, or costly migration behavior.

Current non-negotiable boundaries:

- F# owns static publishing;
- Forma owns reusable presentation;
- Ordo owns engineering constraints;
- Praxis owns work/evidence/provenance;
- Limen remains absent until browser-side application state creates a real need;
- generated output is an artifact, not committed source;
- historical public routes are compatibility contracts.
