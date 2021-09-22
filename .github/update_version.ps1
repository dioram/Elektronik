$global:el=cat .\ProjectSettings\ProjectSettings.asset | Select-String -Pattern '(?:bundleVersion: )((\d*\.\d*\.\d*)(?:-rc\d*)?)' | % {$_.Matches.Groups[2].Value}
cat .\ProjectSettings\ProjectSettings.asset | Foreach {$_ -replace 'bundleVersion: \d*\.\d*\.\d*(?:-rc\d*)?', "bundleVersion: $el"} | Set-Content ps.txt
move -Force ps.txt .\ProjectSettings\ProjectSettings.asset