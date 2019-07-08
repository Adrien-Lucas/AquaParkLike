# AquaParkLike
A copy of AquaPark from Voodoo

## Introduction

As I first played Aquapark, i deciced to make a simple list of the main features of the game that i'll had to make in the Aquapark copy.

* The player can slide on left or right
* AIs are simply taking random tilt on the toboggan
* When a character collides on the back of an other, it boosts the one that was touched
* When a character collides on the side of an other, it throw the touched character on the opposite side
* When a character goes to near from the edge of the toboggan, he is ejected and can fly
* A progress bar shows where the player is on the race circuit
* A text shows the position of the player compared to the other characters

## Technical choices

### GitFlow

In order to have the cleanest git as possible, I used the gitFlow technic. It consists in having a branch per feature and limiting merges.
Working this way forced me to be a lot more focused on a single feature at a time.


![Gitflow example](https://imgur.com/WTFNHFm.png)

### Toboggan Generator

I made simple toboggans module on Blender that are assembled randomly from the pool. The last module is always the "lbby" module where the race begins.
The biggest problem I met was indicating where the player should look when sliding on the toboggan.

#### A waypoint path

I decided to make a path on each module. I added some gizmos to make the task easier.
![Gizmos](https://imgur.com/1fIaQC8.png)

When the toboggan is generated, all the paths are combinated in a single path.
Each character follows the path, segment by segment, and the "graphical" character is placed on the toboggan on top of the point following the path with a raycast.

### Generic character

I tried to make a very generic character first, that does not depend on being the player or an AI.
The character is fully working alone, the AI or Player scripts only gives him inputs.
Nested prefabs were used for that.

#### Not using rigidbody

The way I build the game, using rigidbody would bring a lots of problems. The player is not located in an euclidian way (x, y, z), but with two parameters : Progress on circuit, and deviation form center.
This way, using rigidbody would have messed up because, for example, when a character collides on the side of an other, the deviation should be increased, but rigidbody don't treat it this way.
I had to build a homemade collision system that impacts progress and deviation instead of x, y, z.

### Additional feature

I add a simply "mushroom" button. When you click on it the character is boosted for 3 seconds. I wanted to add collectable boxes (like in MarioKart) but due to the lack of time I had I just gave 2 mushroom to the player and it's not possible to get any more during the game.
Seeing how the game works, a lots of MarioKart features could be add in the game to make it more fun.

# Improvement ideas

I saw that AquaPark was very easy, when the player never touch the screen he often have a very good score.
The AI should be fully reworked.
That's why I think it would be very necessary to use maths in order to mark an analytical solution that founds where the AI as to go when flying to reach a toboggan again.

# Conclusion

Making this remake of Aquapark was a very rich experience. I'm not very proud of the result as the game is not very fluid and has a lots of bugs. I wasn't able to spend the full week on working on making this Aquapark copy, but it would be interesting to go further and make a more playable game.
