dir
Write-Host "Num Args:" $args.Length;
foreach ($arg in $args)
{
  Write-Host "Arg: $arg";
}
Write-Host "Press any key to continue ..."

$x = $host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")

Write-Host
Write-Host "A"
Write-Host "B"
Write-Host "C"

