$ErrorActionPreference = 'SilentlyContinue'

$output = New-Object Collections.Generic.List[object]
$pluginBlacklistUrl = "https://goatcorp.github.io/DalamudAssets/UIRes/bannedplugin.json"

$wc = New-Object system.Net.WebClient
$blackList = $wc.downloadString($pluginBlacklistUrl) | ConvertFrom-Json

$dlTemplate = "https://raw.githubusercontent.com/mousseng/xivtools/main/plugins/{0}/latest.zip"
$dlCount = 0
$apiLevel = 5
$thisPath = Get-Location

Get-ChildItem -Path plugins -File -Recurse -Include *.json |
Foreach-Object {
    $content = Get-Content $_.FullName | ConvertFrom-Json
    $content | add-member -Force -Name "IsHide" -value "False" -MemberType NoteProperty

    $testingPath = Join-Path $thisPath -ChildPath "testing" | Join-Path -ChildPath $content.InternalName | Join-Path -ChildPath $_.Name
    if ($testingPath | Test-Path)
    {
        $testingContent = Get-Content $testingPath | ConvertFrom-Json
        $content | add-member -Force -Name "TestingAssemblyVersion" -value $testingContent.AssemblyVersion -MemberType NoteProperty
    }
    $content | add-member -Force -Name "IsTestingExclusive" -value "False" -MemberType NoteProperty
    $content | add-member -Force -Name "DownloadCount" $dlCount -MemberType NoteProperty
    
    $internalName = $content.InternalName
    
    $updateDate = git log -1 --pretty="format:%ct" plugins/$internalName/latest.zip
    if ($updateDate -eq $null)
    {
        $updateDate = 0;
    }
    $content | add-member -Force -Name "LastUpdate" $updateDate -MemberType NoteProperty

    $installLink = $dlTemplate -f $internalName
    $content | add-member -Force -Name "DownloadLinkInstall" $installLink -MemberType NoteProperty
    
    $installLink = $dlTemplate -f $internalName
    $content | add-member -Force -Name "DownloadLinkTesting" $installLink -MemberType NoteProperty
    
    $updateLink = $dlTemplate -f $internalName
    $content | add-member -Force -Name "DownloadLinkUpdate" $updateLink -MemberType NoteProperty

    $output.Add($content)
}

Get-ChildItem -Path testing -File -Recurse -Include *.json |
Foreach-Object {
    $content = Get-Content $_.FullName | ConvertFrom-Json
    $content | add-member -Force -Name "IsHide" -value "False" -MemberType NoteProperty
    $content | add-member -Force -Name "DownloadCount" $dlCount -MemberType NoteProperty

    if (($output | Where-Object {$_.InternalName -eq $content.InternalName}).Count -eq 0)
    {
        $content | add-member -Force -Name "TestingAssemblyVersion" -value $content.AssemblyVersion -MemberType NoteProperty
        $content | add-member -Force -Name "IsTestingExclusive" -value "True" -MemberType NoteProperty

        $internalName = $content.InternalName
        
        $updateDate = git log -1 --pretty="format:%ct" testing/$internalName/latest.zip
        if ($updateDate -eq $null)
        {
            $updateDate = 0;
        }
        $content | add-member -Force -Name "LastUpdate" $updateDate -MemberType NoteProperty

        $installLink = $dlTemplate -f $internalName
        $content | add-member -Force -Name "DownloadLinkInstall" $installLink -MemberType NoteProperty

        $installLink = $dlTemplate -f $internalName
        $content | add-member -Force -Name "DownloadLinkTesting" $installLink -MemberType NoteProperty

        $updateLink = $dlTemplate -f $internalName
        $content | add-member -Force -Name "DownloadLinkUpdate" $updateLink -MemberType NoteProperty
    
        $output.Add($content)
    }
}

$outputStr = $output | ConvertTo-Json
Out-File -FilePath .\pluginmaster.json -InputObject $outputStr
