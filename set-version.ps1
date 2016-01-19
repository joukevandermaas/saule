$isRelease = $Env:APPVEYOR_REPO_BRANCH -match 'release-v.+';

$project = 'Saule';
$path = Resolve-Path "$project/$project.nuspec";

Write-Host "Updating nuget version for project $project";

[xml]$nuspec = Get-Content $path;
if ($isRelease) {
  $nuspec.package.metadata.version = '$version$';
  Write-Host 'Set version to "$version$"';
} else {
  $nuspec.package.metadata.version = '$version$-beta';
  Write-Host 'Set version to "$version$"-beta';
}

$nuspec.Save($path);
Write-Host "Saved file $path";
