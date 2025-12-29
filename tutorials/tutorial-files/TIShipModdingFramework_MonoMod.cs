using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using TIShipModdingFramework_MonoMod;

#pragma warning disable CS0108
#pragma warning disable CS0626
#pragma warning disable CS0114
#pragma warning disable CS0649

// code provided freely by Razorback and adapted to MonoMod
// https://github.com/UNNRazorback/TIShipModdingFramework

namespace TIShipModdingFramework_MonoMod
{
    public class ShipHull
    {
        private string modBuildName = "";
        public string getModBuildName
        {
            get { return this.modBuildName; }
        }

        private string driveName = "";
        public string getDriveName
        {
            get { return this.driveName; }
        }

        private string hullName = "";
        public string getHullName
        {
            get { return this.hullName; }
        }

        /// <summary>
        /// Initializes the ShipHull class
        /// </summary>
        /// <param name="modBuildName">The name of the mod build from the unity project</param>
        /// <param name="driveName">The name of the given drive</param>
        /// <param name="hullName">The name of the hull from the json file</param>
        public ShipHull(string modBuildName, string driveName, string hullName)
        {
            this.modBuildName = modBuildName;
            this.driveName = driveName;
            this.hullName = hullName;
        }
    }

    public static class ClassReferences
    {
        static List<ShipHull> shipHulls = new List<ShipHull>();
        public static List<ShipHull> getShipHulls
        {
            get { return shipHulls; }
        }

        /// <summary>
        /// Adds a ship hull to the class references list
        /// </summary>
        /// <param name="mod_build_name">The mod build name form the unity project</param>
        /// <param name="json_hull_name">The name of the ship from the json file</param>
        /// <param name="ship_drive_name">The name of the ships drive game object</param>
        public static void AddHull(string mod_build_name, string json_hull_name, string ship_drive_name)
        {
            shipHulls.Add(new ShipHull(mod_build_name, ship_drive_name, json_hull_name));
        }

        public static void ClearHulls()
        {
            shipHulls = new List<ShipHull>();
        }

        /// <summary>
        /// Searches through the array and returns a given index
        /// </summary>
        /// <param name="name">The name of the ship you want to find</param>
        /// <returns>-1 if it cannot find the ship. Otherwise it returns the given index from shipHulls</returns>
        public static int GetShipHullFromName(string name)
        {
            // loops through and searches to find the name
            for (int i = 0; i < getShipHulls.Count; i++)
            {
                if (getShipHulls[i].getHullName.Contains(name))
                {
                    return i;
                }
            }

            //if it doesnt it returns -1
            return -1;
        }

        /// <summary>
        /// Returns a boolean value depending if the ship is in the shipHulls list
        /// </summary>
        /// <param name="name">the name of the ship you want to find</param>
        /// <returns>true if it can find the ship. Otherwise it returns false</returns>
        public static bool DoesShipHullsContainName(string name)
        {
            if (GetShipHullFromName(name) == -1)
                return false;
            return true;
        }
    }
}

namespace PavonisInteractive.TerraInvicta
{
    public abstract class patch_ShipModelController : ShipModelController
    {
        #region Building the Ship

        private extern void orig_BuildWeapons(ShipVisController parentController, TISpaceShipTemplate ship, TISpaceShipState shipState = null);
        private void BuildWeapons(ShipVisController parentController, TISpaceShipTemplate ship, TISpaceShipState shipState = null)
        {
            if (ClassReferences.DoesShipHullsContainName(ship.hullName))
            {

                Log.Debug("===Building Weapon===");
                Log.Debug("All weapon objects: " + allWeaponControllers.Count);
                Log.Debug("CHECKING ALL BASE OBJECTS");
                foreach (ShipWeaponVisController controller in allWeaponControllers)
                {
                    if (controller.baseObject != null)
                        Log.Debug("Base object is not null !");
                    else
                    {
                        controller.baseObject = controller.gameObject;
                        // find weapon object
                        if (controller.weaponObject == null)
                        {
                            Log.Debug("Set controller weapon object");
                            controller.weaponObject = controller.transform.GetChild(0).gameObject;
                        }
                        if (controller.firePoint == null)
                        {
                            Log.Debug("Set controller fire point object");
                            controller.firePoint = controller.weaponObject.transform.GetChild(0).gameObject;
                        }
                        // split name list

                    }

                }

            }
            orig_BuildWeapons(parentController, ship, shipState);
        }

        private extern void orig_BuildDrives(TISpaceShipTemplate ship);
        private void BuildDrives(TISpaceShipTemplate ship)
        {
            if (ClassReferences.DoesShipHullsContainName(ship.hullName))
            {
                Log.Debug("======== " + ship.hullName + " ========");

                // set drive template
                if (ship.driveTemplate != null)
                {
                    Log.Debug("DRIVE TEMPLATE IS NOT NULL!");

                    // Change Thruster Value Here
                    Log.Debug("CHANGING THRUSTER VALUE!");

                    Log.Debug("-- BUILDING DRIVES --");


                    ShipHull hull = null;

                    //get the hull for the name ! 
                    hull = ClassReferences.getShipHulls[ClassReferences.GetShipHullFromName(ship.hullName)];

                    /*
                    GameObject gameObject = GameControl.assetLoader.LoadAsset<GameObject>(hull.getHullName + "/Drive");
                    if (gameObject == null)
                    {
                        Log.Debug("Drive Game Object does not exist ! ! ! ! !");
                    }
                    */
                    // basic drive object

                    string drive_name_string = hull.getDriveName;
                    //sets thruster locations
                    if (thrusterLocations.Count() == 0)
                    {
                        foreach (Transform child in gameObject.transform.Find("Drive").transform)
                        {
                            Log.Debug("Thruster point name=" + child.transform.gameObject.name);
                            if (child.transform.gameObject.name.Contains("ThrusterPoint"))
                            {
                                Log.Debug("Adding thruster point!");
                                thrusterLocations = thrusterLocations.Append(child.gameObject).ToArray();
                            }
                        }
                    }

                    //setting drive model
                    if (thrusterModel == null)
                    {
                        Log.Debug("THRUSTER MODEL IS NULL");
                        GetComponent<HumanShipController>().thrusterModel = gameObject.transform.Find("Drive").gameObject;
                        Log.Debug("DRIVE = " + gameObject.transform.Find("Drive").gameObject);
                    }


                    Log.Debug("Thrusters =" + ship.driveTemplate.thrusters);
                    int thrusters = thrusterLocations.Count();


                    SetDrive(
                        hull.getModBuildName + "/" + drive_name_string,
                        thrusterModel,
                        thrusters * ship.hullTemplate.thrusterMultiplier,
                        ship.driveTemplate,
                        ship.designingFaction,
                        false,
                        ship.hullAppearanceIndex,
                        ship.hullTemplate.simpleHull
                    );

                    Log.Debug("Set Set Drive Fucntion");

                    thrusterModel.SetActive(true);
                }
                else
                {
                    Log.Debug("Drive template is null !! ! ! ! !");
                    Log.Debug("DRIVE NAME = " + ship.driveName);
                }

                //set the thruster value
                thrusters = 0;

                if (ship.driveTemplate != null)
                {
                    return;
                }
            }

            orig_BuildDrives(ship);
        }

        public extern void orig_SetVectorThrusters(TIDriveTemplate drive, TIFactionState faction);
        public void SetVectorThrusters(TIDriveTemplate drive, TIFactionState faction)
        {
            bool modded = false;
            //if it is a vanilla ship
            for (int i = 0; i < ClassReferences.getShipHulls.Count; i++)
            {
                if (name.Contains(ClassReferences.getShipHulls[i].getHullName))
                {
                    modded = true;
                    break;
                }
            }

            if (!modded)
                return;

            Log.Debug("---- VECTOR THRUSTERS ---");
            Log.Debug("SHIP NAME = " + name);
            Log.Debug("IS MODDED SHIP? = " + modded);
            //add vector thrusters if it does not already exist
            if (vectorThrusterGOs.Count() == 0 && modded == true)
            {
                Log.Debug("--------ADDING VECTOR THRUSTERS-------");
                foreach (Transform child in transform)
                {
                    if (child.gameObject.name.Contains("Vector"))
                    {
                        Log.Debug("   -added vector thruster");
                        //Check if the mode is Cinematic or realistic
                        vectorThrusterGOs.Add(child.gameObject);
                    }
                }
            }

            orig_SetVectorThrusters(drive, faction);
        }

        #endregion

        #region Patching for Combat

        public extern void orig_InitDamageLayer();
        public void InitDamageLayer()
        {
            bool modded = false;
            for (int i = 0; i < ClassReferences.getShipHulls.Count; i++)
            {
                if (name.ToLower().Contains(ClassReferences.getShipHulls[i].getHullName.ToLower()))
                {
                    modded = true;
                    break;
                }
            }

            if (modded)
            {
                if (damageLayer == null)
                {
                    // set damage layer
                    Log.Debug("Setting damage layer");
                    //grab the damage layer script from the ships gameobjects
                    damageLayer = gameObject.GetComponent<DamageLayer>();
                    if (damageLayer != null)
                        Log.Debug("Succesfully set damage layer");
                    else
                        Log.Debug("could not set damage layer");
                }
            }

            orig_InitDamageLayer();
        }

        private extern void orig_OnRadiatorDestroyed(ShipRadiatorDestroyed e);
        private void OnRadiatorDestroyed(ShipRadiatorDestroyed e)
        {
            if (!ClassReferences.DoesShipHullsContainName(name))
            {
                orig_OnRadiatorDestroyed(e);
            }
        }

        #endregion

        #region Thrusters

        public extern void orig_ActivateThrusters(bool playAudio);
        public void ActivateThrusters(bool playAudio)
        {
            bool modded_ship = false;

            for (int i = 0; i < ClassReferences.getShipHulls.Count; i++)
            {
                if (name.Contains(ClassReferences.getShipHulls[i].getHullName))
                {
                    modded_ship = true;
                    break;
                }
            }

            if (modded_ship)
            {
                Log.Debug("GETTING THRUSTER FOR " + name);
                // loop through all thursters and deactivate
                foreach (GameObject child in thrusterLocations)
                {

                    // activate thruster particle effect
                    Log.Debug("GOT THRUSTER OBJECT");

                    if (child.transform.GetChild(0) != null)
                        child.transform.GetChild(0).gameObject.SetActive(true);
                }

            }

            orig_ActivateThrusters(playAudio);
        }

        public extern void orig_DeactivateThrusters();
        public void DeactivateThrusters()
        {
            bool modded_ship = false;

            // check if it exst
            for (int i = 0; i < ClassReferences.getShipHulls.Count; i++)
            {
                if (name.Contains(ClassReferences.getShipHulls[i].getHullName))
                {
                    modded_ship = true;
                    break;
                }
            }

            if (modded_ship)
            {
                // loop through all thursters and deactivate
                foreach (GameObject child in thrusterLocations)
                {
                    // activate thruster particle effect
                    if (child.transform.GetChild(0) != null)
                        child.transform.GetChild(0).gameObject.SetActive(false);
                }
            }

            orig_DeactivateThrusters();
        }

        #endregion
    }


    public abstract class patch_HumanShipController : HumanShipController
    {
        public extern void orig_SetSkin(TISpaceShipTemplate ship);
        public override void SetSkin(TISpaceShipTemplate ship)
        {
            if (ship == null)
            {
                Log.Error("Ship template does not exist!");
            }

            if (ship == null || !ClassReferences.DoesShipHullsContainName(ship.hullName))
            {
                orig_SetSkin(ship);
            }
            else
            {
                // finding hull model game object
                Log.Debug(" ADDING HULL MODEL");

                Log.Debug("GAMEOBJECT NAME = " + gameObject.name);

                Log.Debug("---------- ADDING OBJECTS TO INSTANCE -----------");

                // ADDING MODELS
                hullModel = gameObject.transform.Find("Hull").gameObject;
                Log.Debug("HULL = " + gameObject.transform.Find("Hull").gameObject.name);

                // adding drive
                hullModel = gameObject.transform.Find("Drive").gameObject;
                Log.Debug("DRIVE = " + gameObject.transform.Find("Drive").gameObject);


                // normal radiators
                radiator12 = gameObject.transform.Find("Radiator12").gameObject;
                Log.Debug("RADIATOR = " + gameObject.transform.Find("Radiator12").gameObject.name);

                radiator130 = gameObject.transform.Find("Radiator130").gameObject;
                Log.Debug("RADIATOR = " + gameObject.transform.Find("Radiator130").gameObject.name);

                radiator3 = gameObject.transform.Find("Radiator3").gameObject;
                Log.Debug("RADIATOR = " + gameObject.transform.Find("Radiator3").gameObject.name);

                radiator4 = gameObject.transform.Find("Radiator4").gameObject;
                Log.Debug("RADIATOR = " + gameObject.transform.Find("Radiator4").gameObject.name);

                radiator430 = gameObject.transform.Find("Radiator430").gameObject;
                Log.Debug("RADIATOR = " + gameObject.transform.Find("Radiator430").gameObject.name);

                radiator6 = gameObject.transform.Find("Radiator6").gameObject;
                Log.Debug("RADIATOR = " + gameObject.transform.Find("Radiator6").gameObject.name);

                radiator730 = gameObject.transform.Find("Radiator730").gameObject;
                Log.Debug("RADIATOR = " + gameObject.transform.Find("Radiator730").gameObject.name);

                radiator8 = gameObject.transform.Find("Radiator8").gameObject;
                Log.Debug("RADIATOR = " + gameObject.transform.Find("Radiator8").gameObject.name);

                radiator9 = gameObject.transform.Find("Radiator9").gameObject;
                Log.Debug("RADIATOR = " + gameObject.transform.Find("Radiator9").gameObject.name);

                radiator1030 = gameObject.transform.Find("Radiator1030").gameObject;
                Log.Debug("RADIATOR = " + gameObject.transform.Find("Radiator1030").gameObject.name);

                // spike
                spikesRadiator12 = gameObject.transform.Find("spikes 12").gameObject;
                Log.Debug("RADIATOR = " + gameObject.transform.Find("spikes 12").gameObject.name);

                spikesRadiator3 = gameObject.transform.Find("spikes 3").gameObject;
                Log.Debug("RADIATOR = " + gameObject.transform.Find("spikes 3").gameObject.name);

                spikesRadiator6 = gameObject.transform.Find("spikes 6").gameObject;
                Log.Debug("RADIATOR = " + gameObject.transform.Find("spikes 6").gameObject.name);

                spikesRadiator9 = gameObject.transform.Find("spikes 9").gameObject;
                Log.Debug("RADIATOR = " + gameObject.transform.Find("spikes 9").gameObject.name);
                // droplet
                dropletRadiator12 = gameObject.transform.Find("Droplet12").gameObject;
                Log.Debug("RADIATOR = " + gameObject.transform.Find("Droplet12").gameObject.name);
                dropletRadiator8 = gameObject.transform.Find("Droplet8").gameObject;
                Log.Debug("RADIATOR = " + gameObject.transform.Find("Droplet8").gameObject.name);
                dropletRadiator4 = gameObject.transform.Find("Droplet4").gameObject;
                Log.Debug("RADIATOR = " + gameObject.transform.Find("Droplet4").gameObject.name);


                // SELECTION RETICLE
                Log.Debug("ADDING PADLOCK ICONS");
                selectionAnimObject = gameObject.transform.Find("SelectionReticle").gameObject;
                groupSelectionAnimObject = gameObject.transform.Find("GroupSelectionReticle").gameObject;
                padlockIconObject = gameObject.transform.Find("Padlock Container").gameObject;

                Log.Debug("---------- FINISHED ADDING OBJECTS TO INSTANCE -----------");

                // setting drive name
                if (ship.driveTemplate == null)
                    ship.driveName = "ApexSolidRocketx1";
                // ADD WEAPONS  
                Log.Debug("----------- ADDING WEAPONS -----------");
                foreach (Transform child in transform)
                {
                    // if it is a nose weapon
                    if (child.gameObject.name.ToLower().Contains("nose"))
                    {
                        noseWeaponControllers.Add(child.GetComponent<ShipWeaponVisController>());
                        Log.Debug("Added Nose Weapon");
                    }
                    if (child.gameObject.name.ToLower().Contains("dorsal"))
                    {
                        dorsalHullWeaponControllers.Add(child.GetComponent<ShipWeaponVisController>());
                        Log.Debug("Added Top Weapon");
                    }
                    if (child.gameObject.name.ToLower().Contains("ventral"))
                    {
                        ventralHullWeaponControllers.Add(child.GetComponent<ShipWeaponVisController>());
                        Log.Debug("Added Bottom Weapon");
                    }
                }

                Log.Debug("------CHECKING WEAPON LIST------");
                Log.Debug("Nose Weapon Controller Count =" + noseWeaponControllers.Count);
                Log.Debug("Lateral Dorsal Controller Count =" + dorsalHullWeaponControllers.Count);
                Log.Debug("Nose Ventral Controller Count =" + ventralHullWeaponControllers.Count);

                Log.Debug("----- ASSIGNING THRUSTER POINTS ----");
                GameObject driveObject = gameObject.transform.Find("Drive").gameObject;
                if (thrusterLocations.Count() == 0)
                {
                    foreach (Transform child in driveObject.transform)
                    {
                        Log.Debug("Thruster point name=" + child.transform.gameObject.name);
                        if (child.transform.gameObject.name.Contains("ThrusterPoint"))
                        {
                            Log.Debug("Adding thruster point!");
                            thrusterLocations = thrusterLocations.Append(child.gameObject).ToArray();
                        }
                    }
                    Log.Debug("Thruster Location size=" + thrusterLocations.Count());

                }
            }
        }
    }

    public class patch_RadiatorVisController : RadiatorVisController
    {
        public extern void orig_OnRadiatorRepaired();
        public void OnRadiatorRepaired()
        {
            Log.Debug("----Patching Radiators Repair----");
            Log.Debug("Getting Ship human ship controller");

            Log.Debug("GAMEOBJECT NAME = " + gameObject.transform.parent.name);

            ShipHull hull = null;

            bool hasShip = false;
            //grab hull form class
            for (int i = 0; i < ClassReferences.getShipHulls.Count; i++)
            {
                Log.Debug("Looped through ship=" + ClassReferences.getShipHulls[i].getHullName);
                if (gameObject.transform.parent.name.ToLower().Contains(ClassReferences.getShipHulls[i].getHullName.ToLower()))
                {
                    if (ClassReferences.getShipHulls[i] != null)
                    {
                        Log.Debug("setting hull reference");
                        hull = ClassReferences.getShipHulls[i];
                        Log.Debug("Grabbed hull" + hull.getHullName);
                        hasShip = true;
                    }
                    else
                    {
                        Log.Debug("The ship hull does not exist !");
                    }
                }

            }
            // this means that its a vanilla ship !
            if (!hasShip)
            {
                Log.Debug("Could not find ship!");
                orig_OnRadiatorRepaired();
                return;
            }

            // set gameobjects
            Log.Debug("Loading radiator model assets");
            GameObject intact_radiator_model = null;
            GameObject destroyed_radiator_model = null;

            // get the radiator model from the asset build
            if (hasShip)
                intact_radiator_model = GameControl.assetLoader.LoadAsset<GameObject>(hull.getModBuildName + "/" + "radiator_model");
            else
                Log.Debug("Could not grab intact radiator model");
            if (hasShip)
                destroyed_radiator_model = GameControl.assetLoader.LoadAsset<GameObject>(hull.getModBuildName + "/" + "radiator_model");
            else
                Log.Debug("Could not grab intact radiator model");

            //set the radiator model
            if (intactRadiatorModel == null)
            {
                Log.Debug("Building intact radiator model");
                intactRadiatorModel = intact_radiator_model;
                return;
            }

            if (destroyedRadiatorModel == null)
            {
                Log.Debug("Building destroyed radiator model");
                destroyedRadiatorModel = destroyed_radiator_model;
                return;
            }

            orig_OnRadiatorRepaired();
        }

        public extern void orig_OnRadiatorDestroyed(bool radiatorsRetracted);
        public void OnRadiatorDestroyed(bool radiatorsRetracted)
        {
            Log.Debug("----Patching Radiators----");
            Log.Debug("Getting Ship human ship controller");


            Log.Debug("GAMEOBJECT NAME = " + gameObject.transform.parent.name);

            ShipHull hull = null;

            bool hasShip = false;
            //grab hull form class
            for (int i = 0; i < ClassReferences.getShipHulls.Count; i++)
            {
                Log.Debug("Looped through ship=" + ClassReferences.getShipHulls[i].getHullName);
                if (gameObject.transform.parent.name.ToLower().Contains(ClassReferences.getShipHulls[i].getHullName.ToLower()))
                {
                    if (ClassReferences.getShipHulls[i] != null)
                    {
                        Log.Debug("setting hull reference");
                        hull = ClassReferences.getShipHulls[i];
                        Log.Debug("Grabbed hull" + hull.getHullName);
                        hasShip = true;
                    }
                    else
                    {
                        Log.Debug("The ship hull does not exist !");
                    }
                }

            }
            // this means that its a vanilla ship !
            if (!hasShip)
            {
                Log.Debug("Could not find ship!");
                orig_OnRadiatorDestroyed(radiatorsRetracted);
                return;
            }

            // set gameobjects
            Log.Debug("Loading radiator model assets");
            GameObject intact_radiator_model = null;
            GameObject destroyed_radiator_model = null;
            GameObject explosion_prefab = null;

            // get the radiator model from the asset build
            if (hasShip)
                intact_radiator_model = GameControl.assetLoader.LoadAsset<GameObject>(hull.getModBuildName + "/" + "radiator_model");
            else
                Log.Debug("Could not grab intact radiator model");
            if (hasShip)
                destroyed_radiator_model = GameControl.assetLoader.LoadAsset<GameObject>(hull.getModBuildName + "/" + "radiator_model");
            else
                Log.Debug("Could not grab intact radiator model");
            if (hasShip)
                explosion_prefab = GameControl.assetLoader.LoadAsset<GameObject>(hull.getModBuildName + "/" + "radiator_model");
            else
                Log.Debug("Could not grab intact radiator model");

            //set the radiator model
            if (intactRadiatorModel == null)
            {
                Log.Debug("Building intact radiator model");
                intactRadiatorModel = intact_radiator_model;
                return;
            }

            if (destroyedRadiatorModel == null)
            {
                Log.Debug("Building destroyed radiator model");
                destroyedRadiatorModel = destroyed_radiator_model;
                return;
            }

            if (explosionPrefab == null)
            {
                Log.Debug("Buidling explosion prefab");
                explosionPrefab = explosion_prefab;
                return;
            }
            orig_OnRadiatorDestroyed(radiatorsRetracted);
        }
    }
}