If you want to interact with Hue Lamps (with Unity Hue but also in general) you have to follow the follow steps.

1. Find Bridges in your Network (i.e. get their ip adresses)
2. Authenticate your application with the bridge (i.e pressing the link button on the bridge and requesting authentication within the next 30 seconds) which will give you
a username 
3. Send changes to the bridge with that user name (i.e change brightness on light 3
to value 250 )

With Unity Hue you can test out a lot of the functionality in the editor (with the buttons on the Hue Bridge Component) or check out the Unity Hue Demo scene for a runtime example.
So the same steps are:

1. 
Editor:
Press „Discover Bridge“ on the Hue Bridge Component. It will take a bit to receive a
response but the current bridge field on the Component should now have at least an ip and id (depending on model), or an error will be logged to the console.
Code:
Call one of the overloads (different error and success callbacks options) of HueBridge.instance.DiscoverBridges. The default is to get a list of all bridges and set current bridge to the first element or to log an error if there are no bridges in the network.

2.
Editor:
If you have the ip of a bridge, fill in Application Name and Device Name in the current bridge field and press „Create User“. The returned generated user name (if the bridge link was pressed recently) which you can use to authenticate all further requests will become visible in the current bridge field (after a bit) or an error will be logged. It’s useful to store that username for further requests
Code:
Call one of the overloads (different error and success callbacks options again) of HueBridge.instance.CreateUser

3. 
Now you have all the information (bridge ip and username) you need to interact
with the bridge and change the lamps. You should store this information so you don’t have to repeat steps 1 and 2. The Hue Demo Scene provides an example of that.

Editor:
Press „Update Lights“ to get a list of the lights connected to your bridge. After a bit the lamps list should be updated and you should see the lamps with their names (i.e living room 1), you can unfold the lamp objects, change their state and press „Set State“ to actually update the lamp
Code:
Call HueBridge.instance.UpdateLights to get a current list of lights, from the on out you can access the list of HueLamp objects with HueBridge.instance.Lights. On the individual HueLamp object you’ll find info such as name, id, type etc. (sometime depending on version) and you can finally set the state of the lamp with lamp.SetState and provide a list of parameters you want changed (see the demo scene and the HueUIRepresentation script for an example). You can also change the name of the lamp, delete it or access a group of lamps at once.

General Considerations:
Don’t call the api too often. The Philipps API Documentation mentions 10 times a second max for lights and 1 time a second for groups. You can find the Philipps API here (have to sign-up to see it but it’s free): http://www.developers.meethue.com/philips-hue-api
