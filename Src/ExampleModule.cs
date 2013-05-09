/*
   Copyright 2012 BlueWall Information Technologies, LLC

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.

 */

//
// Keep your usings in sync with your code and with the prebuild.xml
// Remove any unused references. Add new ones by placing the reference in
// your prebuild.xml, then run the script to get your project.
// After that, you can add your using reference here.
// If you don't use those steps, then you will have errors when you update
// your OpenSim code and run the prebuild script.
//
// If you need help with C# programming, look here: http://www.csharp-station.com/Tutorial.aspx
//
// 

using System;
using System.Collections.Generic;
using System.Reflection;
using log4net;
using Mono.Addins;
using Nini.Config;
using OpenSim.Region.Framework.Interfaces;
using OpenSim.Region.Framework.Scenes;
using OpenSim.Region.CoreModules.Framework.InterfaceCommander;

// Our plugin loader needs the information about our module
[assembly: Addin("ExampleModule", "0.1")]
[assembly: AddinDependency("OpenSim", "0.5")]

namespace ModExampleModule
{

    // Can make an interface or not ...
    // If you are writing a module that replaces a basic core module, then you will build your
    // module off the same interface that the one you will be replacing implements (usually found in OpenSim/Region/Framework/Interfaces)
    // This is here to show what is needed to replace a basic module. If you want to replace
    // a core basic module, then you will use the interface that the basic module implements
    // inthe places where we use IExampleModule here.
    //
    // Most of the time, you will not need to define or use them.
    // Normally, the only one you are required required to implement is ISharedRegionModule
    // or IRegionModule. We are adding ICommandableModule in this example module to let us illustrate
    // how to add a command to the OpenSim console
    //
    public interface IExampleModule
    {
        // Place items here that we want implementations of our interface to adhere to

    }

    [Extension(Path = "/OpenSim/RegionModules", NodeName = "RegionModule")]
    public class ExampleModule : IExampleModule, ISharedRegionModule, ICommandableModule
    {
        #region Fields
        // This is our logger. use m_log.InfoFormat("[ExampleModule]: Logging Something {0}", m_name);
        private static readonly ILog m_log = LogManager.GetLogger (MethodBase.GetCurrentMethod ().DeclaringType);

        // We can place this here to allow / disallow the module depending on circumstances during the initialization
        // Then we can have our other methods refer back to it when we want to decide to do some function
        private bool m_enabled = false;

        // This is a shared module and will service all Scene in the instance. We keep a copy o fthem here
        // In this case is a Dictionary indexed by the RegionID. Could be a List, as well.treeActiveCommand
        private Dictionary<string, Scene> m_Scenes = new Dictionary<string, Scene> ();

        // We will want to keep a copy of our ConfigSource. It will be passed to us on initialization, bet we
        // will want to use it in several places.
        public IConfigSource m_config;

        // This is just an example - here we save a string for our message from our ini file (see Initialize below}
        string m_ExampleMessage = null;

        // We will have our logger to report back to us during the init process. We will maintain a counter
        // to show us the sequence number
        int m_InitCount = 0;

        // We need this if we want to issue commands on the console. Note the name "example"
        // Type help in the console and you will see "example" listed. The commands we implement
        // will be listed when entering help example
        protected Commander m_commander = new Commander("example");
        #endregion

        #region properties
        // We need to return our name when registering with the simulator. We can have it here (easy when making
        // a generic framework that can be reused. Or, it's OK to just have the Name property do it there. It is
        // here as a convenience { see Name below }
        public string m_name = "ExampleModule";
        #endregion properties

        #region ISharedRegionModule implementation
        public void PostInitialise ()
        {
            // We just have this to show the sequence
            m_log.InfoFormat("[ExampleModule]: Running {0} Sequence {1} : Enabled {2}", "PostInitialize", (m_InitCount++).ToString(), m_enabled.ToString());

            // We have to check to see if we're enabled
            // Return if not enabled
            if (!m_enabled)
                return;
        }

        #endregion

        #region ICommander
        // This is needed if we want to issue commands on the console
        public ICommander CommandInterface
        {
            get { return m_commander; }
        }
        #endregion ICommander

        #region IRegionModuleBase implementation
        // At this point, we have not checked the configuration. We need to see if we
        // are enabled and set our m_enabled
        public void Initialise (IConfigSource source)
        {
            // We just have this to show the sequence
            m_log.InfoFormat("[ExampleModule]: Running {0} Sequence {1} : Enabled {2}", "Initialize", (m_InitCount++).ToString(), m_enabled.ToString());

            // We want to save our config source to be able to get it at our convenience
            m_config = source;

            // Put a section in your ini for this ...
            // [ExampleModule]
            //   enabled = true
            //   ExampleMessage = "Welcome to the Metaverse!"
            //
            IConfig cnf = source.Configs["ExampleModule"];
            
            if (cnf == null)
            {
                m_enabled = false;
                m_log.Info ("[ExampleModule]: No Configuration Found, Disabled");
                return;
            }

            // get our enabled state from the ini
            m_enabled = cnf.GetBoolean("enabled", false);

            if (m_enabled == false)
            {
                m_log.InfoFormat ("[ExampleModule]: Module was disabled, {0}", m_ExampleMessage);
                return;
            }

            // get our message from the ini, if we don't define it, then we will assign "Default Message" as the value
            m_ExampleMessage = cnf.GetString ("ExampleMessage", "Default Message");
            if (m_ExampleMessage != "")
            {
                m_log.InfoFormat ("[ExampleModule]: Module was enabled, {0}", m_ExampleMessage);
                m_enabled = true;
            }
        }

        public void Close ()
        {
            // We just have this to show the sequence
            m_log.InfoFormat("[ExampleModule]: Running {0} Sequence {1} : Enabled {2}", "Close", (m_InitCount++).ToString(), m_enabled.ToString());


            // We have to check to see if we're enabled
            // Return if not enabled
            if (!m_enabled)
                return;

        }

        public void AddRegion (Scene scene)
        {
            // We just have this to show the sequence
            // remove from your real module
            m_log.InfoFormat("[ExampleModule]: Running {0} Sequence {1} : Enabled {2}", "AddRegion", (m_InitCount++).ToString(), m_enabled.ToString());

            // We have to check to see if we're enabled
            // Return if not enabled
            if (!m_enabled)
                return;

            // Here we copy our Scene to our Dictionary
            m_log.InfoFormat ("[ExampleModule]: Adding {0}", scene.RegionInfo.RegionName);
            if (m_enabled == true)
            {
                if (m_Scenes.ContainsKey (scene.RegionInfo.RegionName)) {
                    lock (m_Scenes)
                    {
                        m_Scenes[scene.RegionInfo.RegionName] = scene;
                    }
                } else {
                    lock (m_Scenes)
                    {
                        m_Scenes.Add (scene.RegionInfo.RegionName, scene);
                    }
                }

                // Hook up events
                // This will fire when a client enters the region
                scene.EventManager.OnNewClient += OnNewClient;
                // This will fire when we type our command into the console
                scene.EventManager.OnPluginConsole += HandleSceneEventManagerOnPluginConsole;


                // Take ownership of the IExampleModule service
                // This is optional for external modules.
                // But, for custom modules that are designed to replace basic ones,
                // you will register the interface with the module loader so this one
                // is loaded instead of the basic module.
                // If your module intends to be replaceable, then return this interface
                // type (See RepleableInterface property below)
                // If you module is replaceing, then register the interface here
                // and return null with the ReplaceableInterface property
                scene.RegisterModuleInterface<IExampleModule>(this);


                // The following are optional and is part of the required components to create
                // console commands to be handled by our module
                //
                // Add a command to set the message in the console
                // See the handler 'HandleSetMessage' below
                Command set_message = new Command("set-message", CommandIntentions.COMMAND_NON_HAZARDOUS, HandleSetMessage, "Set ExampleModule message");
                set_message.AddArgument("message", "the message", "String");
                m_commander.RegisterCommand("set-message", set_message);

                // Add a command to get the message in the console
                // See the handler 'HandleGetMessage' below
                Command get_message = new Command("get-message", CommandIntentions.COMMAND_NON_HAZARDOUS, HandleGetMessage, "Get ExampleModule message");
                m_commander.RegisterCommand("get-message",get_message);

                // Register our command handler
                scene.RegisterModuleCommander(m_commander);
            }
        }

        public void RemoveRegion (Scene scene)
        {
            // We just have this to show the sequence
            m_log.InfoFormat("[ExampleModule]: Running {0} Sequence {1} : Enabled {2}", "RemoveRegion", (m_InitCount++).ToString(), m_enabled.ToString());

            // We have to check to see if we're enabled
            // Return if not enabled
            if (!m_enabled)
                return;

            // un-register our event handlers...
            scene.EventManager.OnNewClient -= OnNewClient;
            scene.EventManager.OnPluginConsole -= HandleSceneEventManagerOnPluginConsole;

            // We can remove this Scene from out Dictionary - it may have others
            // if we run more than one region in the instnce
            if (m_Scenes.ContainsKey (scene.RegionInfo.RegionName)) {
                lock (m_Scenes)
                {
                    m_Scenes.Remove (scene.RegionInfo.RegionName);
                }
            }
            scene.UnregisterModuleInterface<IExampleModule>(this);
        }

        public void RegionLoaded (Scene scene)
        {
            // We just have this to show the sequence
            // Remove from your real module
            m_log.InfoFormat("[ExampleModule]: Running {0} Sequence {1} : Enabled {2}", "RegionLoaded", (m_InitCount++).ToString(), m_enabled.ToString());

            // We have to check to see if we're enabled
            // Return if not enabled
            if (!m_enabled)
                return;
        }

        public string Name {

            get
            {
                // We just have this to show the sequence:
                // Not a good practice to run code in a property so remove it when you make your modules
                m_log.InfoFormat("[ExampleModule]: Running {0} Sequence {1} : Enabled {2}", "Name", (m_InitCount++).ToString(), m_enabled.ToString());
                return m_name;
            }
        }

        public Type ReplaceableInterface {

            get
            {
                // We just have this to show the sequence: Not a good practice to run code in a property
                // Remove from your real module
                m_log.InfoFormat("[ExampleModule]: Running {0} Sequence {1} : Enabled {2}", "ReplaceableInterface", (m_InitCount++).ToString(), m_enabled.ToString());

                // If we want to load over another module with a custom one,
                // we return null. If we are writing one that we will allow to be replaced,
                // then we would do this -> return typeof(IExampleModule);
                return null;
            }
        }
        #endregion

        #region Event Handlers
        void OnNewClient (OpenSim.Framework.IClientAPI client)
        {
            // Use our convenience method below to get the client name, then send them a modal alert message
            // we use our m_ExampleConfig for the message - we can set that in the ini
            client.SendAgentAlertMessage(String.Format("Hello! {0}! {1}", GetClientName(client), m_ExampleMessage), true);

            m_log.InfoFormat("[ExampleModule]: NewClient {0} @ {1}", client.Name, client.RemoteEndPoint.ToString());
        }

        // This is needed if we want to issue console commands
        void HandleSceneEventManagerOnPluginConsole (String[] args)
        {
            // note name "example"
            if (args[0] == "example")
            {
                // Check if we want to set the welcome message
                if (args[1] == "set-message")
                {
                    string[] message = new string[args.Length - 2];
                    int i;
                    for (i = 2; i < args.Length; i++)
                    {
                        message[i - 2] = args[i];
                    }

                    m_commander.ProcessConsoleCommand(args[1], message);
                    return;
                }

                // We are wanting to check the welcome message
                if (args[1] == "get-message")
                {
                    string[] msg = new string[0];
                    m_commander.ProcessConsoleCommand(args[1], msg);
                    return;
                }
            }
        }

        private void HandleSetMessage(Object[] args)
        {
            if(!String.IsNullOrEmpty((string)args[0].ToString()))
            {
                m_ExampleMessage = (string)args[0];
                OpenSim.Framework.MainConsole.Instance.Output(String.Format("Message is: {0}",m_ExampleMessage.ToString()));
                return;
            }
        }

        private void HandleGetMessage(Object args)
        {
            OpenSim.Framework.MainConsole.Instance.Output(String.Format("Message is: {0}",m_ExampleMessage.ToString()));
            return;
        }
        #endregion Event Handlers

        #region ExampleModule
        // Constructor
        public ExampleModule ()
        {
            m_log.InfoFormat("[ExampleModule]: Running {0} Sequence {1} : Enabled {2}", "Constructor", (m_InitCount++).ToString(), m_enabled.ToString());
            
        }

        // We can add convenience methods to do tedious tasks and use them
        private string GetClientName(OpenSim.Framework.IClientAPI client)
        {
            return String.Format("{0} {1}", client.FirstName, client.LastName);
        }
        #endregion
    }
}
