$UpdateVersionNumber = $true

$SourceDir = "$env:USERPROFILE\Source\Repos\TonyValenti\msbuild-obfuscar\src\";

$VisualStudio = "C:\Program Files\Microsoft Visual Studio\2022\Professional\"
$TextTransform = "$VisualStudio\Common7\IDE\TextTransform.exe"

$NUGET_DEV = "C:\__DEV\__NUGET";

if($UpdateVersionNumber){
    "Updating Version Number..."
	Start-Process -Wait $TextTransform -ArgumentList @("`"$SourceDir\__SHARED\AssemblyInfo.Internal.tt`"")
    Start-Process -Wait $TextTransform -ArgumentList @("`"$SourceDir\__SHARED\AssemblyInfo.Nuget.tt`"")

}

Get-Process | Where-Object { $_.Name -eq "MSBUILD" } | Kill

$FoldersToRemove = @(
    "$env:USERPROFILE\.nuget\packages\obfuscar.msbuild\",
    "$env:USERPROFILE\source\repos\MediatedCommunications\__BUILD",
    "$env:USERPROFILE\source\repos\MediatedCommunications\UniversalMigrator\__BUILD",
    $NUGET_DEV
    )

foreach($FolderToRemove in $FoldersToRemove){
    if(Test-Path $FolderToRemove){
        rmdir $FolderToRemove -Recurse 
    }
}

mkdir $NUGET_DEV

dotnet build --configuration Release $SourceDir\Obfuscar.MsBuild\
dotnet pack --configuration Release $SourceDir\Obfuscar.MsBuild\

copy "$SourceDir\Obfuscar.MsBuild\bin\Release\*.nupkg" "$NUGET_DEV"