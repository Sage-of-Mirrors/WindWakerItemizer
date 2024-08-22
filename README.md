# Wind Waker Itemizer
<p align="center">
  <img src="https://github.com/user-attachments/assets/bef4869e-dbd5-479d-a0f4-8df577251e34"/>
</p>

Wind Waker Itemizer is a tool for the original GameCube release of <i>The Legend of Zelda: The Wind Waker</i> that allows the user to edit what pickups enemies can drop when they are defeated.

## Item Drops in <i>The Wind Waker</i>

When an enemy actor is killed, the game rolls a random number between 0.00 and 99.99, truncates it to a value between 0 and 99, and compares that to the chance of the actor spawning an item ball; see [the code](https://github.com/user-attachments/assets/bef4869e-dbd5-479d-a0f4-8df577251e34) at the TWW decomp project. If the random value is <i>lower</i> than the item ball chance (`drop_chance > random_value`), the game will spawn an item ball containing all of the items listed in the Itemizer under `Item Ball Contents`.

Otherwise, the game rolls another random number between 0.00 and 15.99, truncates the value to fit between 0 and 15, and uses it to choose an item from the items listed in the Itemizer under `Loose Items`.

To summarize:
* How often an item ball drops is determined by a value between 0 and 100. The higher the number, the more often an item ball will appear. The item ball contains <i>all</i> the items in the `Item Ball Contents` section of the Itemizer.
* If an item ball is not dropped, the game will randomly choose between 16 possible items. The pool of items it can pick from is listed under the `Loose Items` section of the Itemizer.

## Adding Drop Configurations
Adding entirely new item drop configurations is <i>technically</i> possible, but would require the use of the [TWW Decomp Project.](https://github.com/zeldaret/tww). This is because the configuration used by a particular enemy actor is explicitly defined, rather than a consequence of the game choosing the configuration based on the actor's name. Consider this snippet of [code from ReDeads](https://github.com/zeldaret/tww/blob/ce9dde8595adfb38ee3737ce7f1fbe35c9e0019a/src/d/actor/d_a_rd.cpp#L1799):

```cpp
if (mWhichIdleAnm == 0) {
    itemTableIdx = dComIfGp_CharTbl()->GetNameIndex("Rdead1", 0);
}
if (mWhichIdleAnm == 1) {
    itemTableIdx = dComIfGp_CharTbl()->GetNameIndex("Rdead2", 0);
}
```

The actor's `itemTableIdx` is set by calling `dComIfGp_CharTbl()->GetNameIndex()`, which takes the name of the configuration and an integer value. Using a brand-new configuration, for example named `myConfig`, would require the actor to call `dComIfGp_CharTbl()->GetNameIndex("myConfig", 0);` and store the return value to pass to the puff-of-smoke actor when it is killed.
