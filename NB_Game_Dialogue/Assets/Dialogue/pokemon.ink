INCLUDE globals.ink

#audio:animal_crossing_low #speaker:JN

-> main

=== main ===
Young man... 
Prepare yourself to kill the monster.
What do you need?
    + [Sushi]
        -> chosen("Sushi")
    + [Sword]
        -> chosen("Sword")
        
=== chosen(item) ===
{item == "Sushi":
    Here you are... Five fresh sushi.
    ~ sushichange = true
- else:
    Here you are... The best one.
    ~ swordchange = true
}
-> END
