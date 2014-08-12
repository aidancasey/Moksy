param($installPath, $toolsPath, $package, $project)

$file1 = $project.ProjectItems.Item("Moksy.Host.exe.config")

# set 'Copy To Output Directory' to 'Copy always'
$copyToOutput1 = $file1.Properties.Item("CopyToOutputDirectory")
$copyToOutput1.Value = 1
