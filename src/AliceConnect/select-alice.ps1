./AliceConnect connect-audio -n "Станция Мини 3"
Start-Sleep -Seconds 5
./AliceConnect switch-audio -n "Станция Мини 3"
Start-Sleep -Seconds 5
./AliceConnect switch-audio -n "Headphones"
python Python/led_panel_power.py POWER_ON
