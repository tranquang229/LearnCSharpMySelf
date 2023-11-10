// See https://aka.ms/new-console-template for more information
//Console.WriteLine("Hello, World!");

using CommandPattern;

//SimpleRemoteControl remote = new SimpleRemoteControl();

//Light light = new Light();
//LightOnCommand lightOn = new LightOnCommand(light);

//GarageDoor garageDoor = new GarageDoor();
//GarageDoorOpenCommand garageDoorOpenCommand = new GarageDoorOpenCommand(garageDoor);

//remote.SetCommand(lightOn);
//remote.ButtonWasPressed();

//remote.SetCommand(garageDoorOpenCommand);
//remote.ButtonWasPressed();


//RemoteControl remote = new RemoteControl();

//Light livingRoomLight = new Light("Living");
//Light kitchenRoomLight = new Light("Kitchen");

//LightOnCommand livingRoomLightOn = new LightOnCommand(livingRoomLight);
//LightOffCommand livingRoomLightOff = new LightOffCommand(livingRoomLight);

//LightOnCommand kitchenRoomLightOn = new LightOnCommand(kitchenRoomLight);
//LightOffCommand kitchenRoomLightOff = new LightOffCommand(kitchenRoomLight);

//remote.SetCommand(0, livingRoomLightOn, livingRoomLightOff);
//remote.SetCommand(1, kitchenRoomLightOn, kitchenRoomLightOff);

//Console.WriteLine(remote.ToString());

//remote.OnButtonWasPushed(0);
//remote.OffButtonWasPushed(0);

//remote.OnButtonWasPushed(1);
//remote.OffButtonWasPushed(1);




//RemoteControlWithUndo remote = new RemoteControlWithUndo();

//Light livingRoomLight = new Light("Living");

//LightOnCommand livingRoomLightOn = new LightOnCommand(livingRoomLight);
//LightOffCommand livingRoomLightOff = new LightOffCommand(livingRoomLight);

//remote.SetCommand(0, livingRoomLightOn, livingRoomLightOff);
//remote.OnButtonWasPushed(0);
//remote.OffButtonWasPushed(0);
////Console.WriteLine(remote.ToString());

//remote.UndoButtonWasPushed();
////Console.WriteLine(remote.ToString());

//remote.OffButtonWasPushed(0);
//remote.OnButtonWasPushed(0);
////Console.WriteLine(remote.ToString());
//remote.UndoButtonWasPushed();
////Console.WriteLine(remote.ToString());



//RemoteControlWithUndo remoteControl = new RemoteControlWithUndo();
//CeilingFan ceilingFan = new CeilingFan("Living room");
//CeilingFanHighCommand ceilingFanHighCommand = new CeilingFanHighCommand(ceilingFan);
//CeilingFanMediumCommand ceilingFanMediumCommand = new CeilingFanMediumCommand(ceilingFan);
//CeilingFanLowCommand ceilingFanLowCommand = new CeilingFanLowCommand(ceilingFan);
//CeilingFanOffCommand ceilingFanOffCommand = new CeilingFanOffCommand(ceilingFan);

//remoteControl.SetCommand(0, ceilingFanHighCommand, ceilingFanOffCommand);
//remoteControl.SetCommand(1, ceilingFanMediumCommand, ceilingFanOffCommand);

//remoteControl.OnButtonWasPushed(0);
////remoteControl.OffButtonWasPushed(0);
////remoteControl.UndoButtonWasPushed();

//remoteControl.OnButtonWasPushed(1);
////remoteControl.UndoButtonWasPushed();

Light light = new Light("Living room");
TV tv = new TV("Living room");

LightOnCommand lightOnCommand = new LightOnCommand(light);
LightOffCommand lightOffCommand = new LightOffCommand(light);

TVOnCommand tvOnCommand = new TVOnCommand(tv);
TVOffCommand tvOffCommand = new TVOffCommand(tv);

ICommand[] partyOn = { lightOnCommand, tvOnCommand };
ICommand[] partyOff = { lightOffCommand, tvOffCommand };
MarcoCommand partyOnMarco = new MarcoCommand(partyOn);
MarcoCommand partyOffMarco = new MarcoCommand(partyOff);

RemoteControlWithUndo remoteControl = new RemoteControlWithUndo();
remoteControl.SetCommand(0, partyOnMarco, partyOffMarco);

remoteControl.OnButtonWasPushed(0);
remoteControl.OffButtonWasPushed(0);