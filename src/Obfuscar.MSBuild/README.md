# Obfuscar.MsBuild

MSBuild integration for [Obfuscar](https://www.obfuscar.com). Adding this package as a `PackageReference` restores Obfuscar and runs the **Obfuscate** task after **Build**.

This package is a development dependency. It does not ship as a runtime library.

## Install

```bash
dotnet add package Obfuscar.MsBuild
```

Or Package Manager:

```powershell
Install-Package Obfuscar.MsBuild
```

## Usage

No extra `PackageReference` to Obfuscar is required. After restore, build the project. Obfuscation runs automatically.

Optional `Obfuscar.xml` in the project or solution directory is used when present; otherwise a built-in template is applied.

## Properties

| Property | Default | Meaning |
| --- | --- | --- |
| `ObfuscarEnabled` | `true` | Set to `false` to skip obfuscation. |
| `Obfuscator_Targets` | `Debug,Release` | Configurations that are obfuscated. |
| `Obfuscator_Path` | restored `Obfuscar.Console.exe` | Override the Obfuscar executable. |
| `Obfuscar` | set by the Obfuscar package | Alternate path to `Obfuscar.Console.exe`. |
| `Obfuscator_ConfigTemplate` | `Obfuscar.xml` if found | Path to an Obfuscar config template. |

Example: obfuscate Release only.

```xml
<PropertyGroup>
  <Obfuscator_Targets>Release</Obfuscator_Targets>
</PropertyGroup>
```

## Documentation

Obfuscar configuration: [docs.obfuscar.com](https://docs.obfuscar.com/).

## License

MIT. See the [LICENSE](https://github.com/ennerperez/msbuild-obfuscar/blob/master/LICENSE) file in the repository.
