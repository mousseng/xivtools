#$ErrorActionPreference = 'SilentlyContinue'

$output = New-Object Collections.Generic.List[object]
$dlTemplate = "https://raw.githubusercontent.com/mousseng/xivtools/main/plugins/{0}/latest.zip"
$dlCount = 0

Get-ChildItem -Path plugins -File -Recurse -Include *.json |
Foreach-Object {
    $content = Get-Content $_.FullName | ConvertFrom-Json
    
    $internalName = $content.InternalName
    $installLink = $dlTemplate -f $internalName
    $updateDate = git log -1 --pretty="format:%ct" plugins/$internalName/latest.zip
    if ($updateDate -eq $null)
    {
        $updateDate = 0;
    }
    
    $content | add-member -Force -Name "IsHide" -value $false -MemberType NoteProperty
    $content | add-member -Force -Name "IsTestingExclusive" -value $false -MemberType NoteProperty
    $content | add-member -Force -Name "DownloadCount" $dlCount -MemberType NoteProperty
    $content | add-member -Force -Name "LastUpdate" $updateDate -MemberType NoteProperty
    $content | add-member -Force -Name "DownloadLinkInstall" $installLink -MemberType NoteProperty
    $content | add-member -Force -Name "DownloadLinkTesting" $installLink -MemberType NoteProperty
    $content | add-member -Force -Name "DownloadLinkUpdate" $installLink -MemberType NoteProperty

    $output.Add($content)
}

$outputStr = $output | ConvertTo-Json
Out-File -FilePath .\pluginmaster.json -InputObject $outputStr
