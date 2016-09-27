# UnityHue
Small library that lets you manipulate Philips Hue lamps from within the editor. Uses unity webrequests to communicate with
the hue bridge so it should be pretty crossplatform but will only work in Unity 5.2+ ([More on platform support](https://docs.unity3d.com/Manual/UnityWebRequest.html)).

##Functionality: 

- Discover bridges in network
- Authenticate app with bridge (after pressing the link button on the bridge)
- Retrieve list of available lights and light groups
- Rename light, change light state (on/off, color, transition time, alert, effect etc. (pretty much all the api allows)), 
get light state and delete light 
- Create groups, rename groups and change associated lights, delete groups and change group state (like with lamps but affecting all the lamps in the group)

##License:
MIT
