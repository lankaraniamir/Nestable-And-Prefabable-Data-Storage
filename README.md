## Components

This framework consists of three main item types. A data holder which acts as a single location to 
edit all nested data, an external access point for data in-game, and a manager of naming and file
structure for all nested assets. Held within data holders are any number of data nests which are 
normal scriptable objects but can be nested as a child of any other data nest as much as desired. 
Lastly, each data nest is associated with a prefab editor, which can create changes to any component 
within a prefab associated with the data nest whenever the value of a data item is changed.

## Usage

This framework is particularly useful for rapid creation of complex and largely-variadic objects 
of a single type through a data-oriented approach. Data values within one data nest can be used 
to turn on and off other data nests allowing complex and unique data configurations to exist
by specification; similarly, prefabs can be altered to create components or change their field
values based upon such data changds automatically. Thus, instead of having to create singular
scriptable objects containing data for every edge case, only the necessary data for any object
needs to be stored at any time. Furthermore, instead of having to create complex and computationally
heavy scripts to create objects based upon data during runtime, all this setup can be done prior
to start and easily loaded through a single line of code.

## Example

I have provided a small example of such usage based upon the game I created this system for. 
In this example, there is data for various companions all accessible through MyDataHolder
which is accessible in game through the DataLoader static functions. When the shape data
for a companion is changed, the prefab for the companion also has its shape changed. If the 
companion has a projectile-based attack, a prefab is generated for the projectile as well,
which holds the projectile script that uses the same projectile data. With this design,
many companions can be easily created by simply filling in the data 

## Note

The wonderful attribute system [Naughty Attributes](https://github.com/dbrizov/NaughtyAttributes) 
must be downloaded for this to work. With it installed, all data can be edited directly in the
data holder scriptable objects and nests need not be directly accessed.
