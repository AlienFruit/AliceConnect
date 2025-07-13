AliceConnect connect-audio -n "Станция Мини 3"
timeout /t 5 /nobreak
AliceConnect switch-audio -n "Станция Мини 3"
timeout /t 5 /nobreak
AliceConnect switch-audio -n "Headphones"

python Python/led_panel_power.py POWER_ON