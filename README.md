# PiratesQuest 
> A one-hour pirate sandbox where economy, exploration, and combat collide—designed to test how humans and AIs compete, cooperate, and outplay each other in a living system.

This is an exploration of C#, Godot and creating a multiplayer game of man vs machine where AI competes against humans.

## Running 
To run in the editor:
- Run `run.sh --server`
- Run the client in the editor

To run without the editor:
- Run `run.sh`

Please submit a PR, learning godot, so any and all suggestions welcome.

## Releasing
- Update Version in Project Settings
- Run `./build.sh`
- Add new Git Release in github
- Upload builds in dist to github release

## Third Party
- [GoDot](https://godotengine.org/)
- Kenney.nl
  - [Prototype Textures](https://kenney.nl/assets/prototype-textures)
  - [Pirate Pack](https://kenney.nl/assets/pirate-kit)

## Todo for first alpha trial
- [x] Ship movement
- [x] Multiplayer
- [ ] Core Game Mechanics
  - [x] Collection of resources
  - [x] Three resource types & store
  - [x] Shooting Cannonballs, health, and death.
  - [x] Limited Map with islands
  - [ ] Upgrades
- [ ] API for AI characters
  - [ ] Different ship design for AI character?
- [ ] Graphics
  - [ ] Water
  - [x] Opaque collection spaces
  - [ ] Hit Markers
  - [x] Collection spots have island
- [ ] Distribution
