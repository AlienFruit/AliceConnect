./AliceConnect connect-audio -n "Станция Мини 3"
Write-Host ([Environment]::NewLine)
Start-Sleep -Seconds 5

./AliceConnect switch-audio -n "Станция Мини 3"
Write-Host ([Environment]::NewLine)
Start-Sleep -Seconds 5

./AliceConnect switch-audio -n "Headphones"
Write-Host ([Environment]::NewLine)

Write-Host ("Turning on the LED light bar...")
python Python/led_panel_power.py POWER_ON
