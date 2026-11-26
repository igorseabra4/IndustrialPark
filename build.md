## Builds and releases

Pushes and pull requests to `master` build the solution and run the tests on Windows. Local and CI builds are labeled `-beta`.

Use the `package-current.ps1` script to build a release package and create a zip file.

The ZIP includes the application, Randomizer, updater, required resources, Windows native libraries and licenses.

The Linux and macOS native Assimp libraries are omitted: Wine runs Windows binaries, and [.NET selects native assets by runtime identifier](https://learn.microsoft.com/en-us/dotnet/core/rid-catalog).