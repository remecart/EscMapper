using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;


public class MapValidation : MonoBehaviour
{
    public List<string> errors;
    public List<GameObject> objects;
    public List<GameObject> zones;

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            ValidateMap();
        }
    }

    public void ValidateMap()
    {
        GetAllObjects();
        errors.Clear();

        var count = 1;
        var properties = MapProperties.instance.properties;

        if (properties == null) return;


        if (properties.Info.MapName == string.Empty) LogError(count++, "No map name!");
        if (properties.Info.Warden == string.Empty) LogError(count++, "No warden name!");
        if (properties.Info.Intro == string.Empty) LogError(count++, "No warden note!");
        if (GetActiveJobs(properties) < 3) LogError(count++, "Needs a minimum of three jobs!");
        if (GetActiveJobs(properties) > properties.Info.Inmates) LogError(count++, "Too many jobs!");

        if (GetObjectsOfType(54) + GetObjectsOfType(111) != 1) LogError(count++, "Map needs ONE player bed!");
        if (GetObjectsOfType(55) != 1) LogError(count++, "No player desk!");
        if (GetObjectsOfType(1) + GetObjectsOfType(110) < properties.Info.Inmates)
            LogError(count++,
                $"Needs {properties.Info.Inmates - GetObjectsOfType(1) - GetObjectsOfType(110)} more prisoner beds!");
        if (GetObjectsOfType(9) < properties.Info.Inmates)
            LogError(count++, $"Needs {properties.Info.Inmates - GetObjectsOfType(9)} more prisoner desks!");

        if (GetObjectsOfType(16) < properties.Info.Inmates)
            LogError(count++,
                $"Needs {properties.Info.Inmates - GetObjectsOfType(16)} more prisoner rollcall waypoints!");
        if (GetObjectsOfType(21) < properties.Info.Inmates)
            LogError(count++,
                $"Needs {properties.Info.Inmates - GetObjectsOfType(21)} more prisoner shower waypoints!");
        if (GetObjectsOfType(2) < properties.Info.Inmates)
            LogError(count++, $"Needs {properties.Info.Inmates - GetObjectsOfType(2)} more canteen chairs!");
        if (GetObjectsOfType(19) < 5)
            LogError(count++, $"Needs {5 - GetObjectsOfType(19)} more guard roam waypoints!");
        if (GetObjectsOfType(18) < 5)
            LogError(count++, $"Needs {5 - GetObjectsOfType(18)} more prisoner roam waypoints!");
        if (GetObjectsOfType(17) < 3)
            LogError(count++, $"Needs {3 - GetObjectsOfType(17)} more guard rollcall waypoints!");
        if (GetObjectsOfType(22) < 3)
            LogError(count++, $"Needs {3 - GetObjectsOfType(22)} more guard canteen waypoints!");
        if (GetObjectsOfType(14) < 3)
            LogError(count++, $"Needs {3 - GetObjectsOfType(14)} more guard shower waypoints!");
        if (GetObjectsOfType(39) < 3)
            LogError(count++, $"Needs {3 - GetObjectsOfType(39)} more guard gym waypoints!");
        if (GetObjectsOfType(72) != 1) LogError(count++, $"Map should have ONE NPC spawn waypoint!");
        if (GetObjectsOfType(73) != 1) LogError(count++, $"Map should have ONE NPC employment waypoint!");
        if (GetObjectsOfType(94) != 1) LogError(count++, $"Map should have ONE NPC medic waypoint!");

        if (GetObjectsOfType(56) < properties.Info.Guards)
            LogError(count++, $"Needs {properties.Info.Guards - GetObjectsOfType(56)} more guard beds!");
        if (GetObjectsOfType(19) == 0) LogError(count++, $"Needs NPC prisoner food collection waypoint!");
        if (GetObjectsOfType(59) == 0) LogError(count++, $"No solitary bed!");
        if (GetObjectsOfType(12) == 0) LogError(count++, $"No imfirmary bed!");
        if (GetObjectsOfType(6) + GetObjectsOfType(69) == 0)
            LogError(count++, $"No internet terminals or bookcases for intellect raising!");
        if (GetObjectsOfType(49) == 0) LogError(count++, $"Map has no light sources set!");

        var gymEquipmentCount = GetObjectsOfType(7) + GetObjectsOfType(8) + GetObjectsOfType(102) +
                                GetObjectsOfType(91) + GetObjectsOfType(90) + GetObjectsOfType(96) +
                                GetObjectsOfType(97) + GetObjectsOfType(98);
        if (gymEquipmentCount < properties.Info.Inmates)
            LogError(count++, $"Needs {properties.Info.Inmates - gymEquipmentCount} workout equipments!");
        if (GetObjectsOfType(11) < 3) LogError(count++, $"Needs at leat 3 canten food trays!");

        if (properties.Jobs.Janitor)
        {
            if (GetObjectsOfType(47) == 0) LogError(count++, $"Janitor: No cleaning supplies!");
            if (GetZoneOfType("Janitor")) LogError(count++, $"Janitor zone is missing!");
        }

        if (properties.Jobs.Gardening)
        {
            if (GetObjectsOfType(89) == 0) LogError(count++, $"Gardening: No garden tools container!");
            if (GetZoneOfType("Gardening")) LogError(count++, $"Gardening zone is missing!");
        }

        if (properties.Jobs.Laundry)
        {
            if (GetObjectsOfType(5) == 0) LogError(count++, $"Laundry: No washing machine!");
            if (GetObjectsOfType(35) == 0) LogError(count++, $"Laundry: No dirty laundry container!");
            if (GetObjectsOfType(36) == 0) LogError(count++, $"Laundry: No clean laundry container!");
            if (GetZoneOfType("Laundry")) LogError(count++, $"Laundry zone is missing!");
        }

        if (properties.Jobs.Kitchen)
        {
            if (GetObjectsOfType(10) == 0) LogError(count++, $"Kitchen: No freezer!");
            if (GetObjectsOfType(4) == 0) LogError(count++, $"Kitchen: No oven!");
            if (GetZoneOfType("Kitchen")) LogError(count++, $"Kitchen zone is missing!");
        }

        if (properties.Jobs.Tailor)
        {
            if (GetObjectsOfType(79) == 0) LogError(count++, $"Tailor: No fabric container!");
            if (GetObjectsOfType(80) == 0) LogError(count++, $"Tailor: No clothing container!");
            if (GetZoneOfType("Tailor")) LogError(count++, $"Tailor zone is missing!");
        }

        if (properties.Jobs.Woodshop)
        {
            if (GetObjectsOfType(41) == 0) LogError(count++, $"Woodshop: No timber container!");
            if (GetObjectsOfType(42) == 0) LogError(count++, $"Woodshop: No furniture container!");
            if (GetZoneOfType("Woodshop")) LogError(count++, $"Woodshop zone is missing!");
        }

        if (properties.Jobs.Library)
        {
            if (GetObjectsOfType(81) == 0) LogError(count++, $"Library: No book chest!");
        }

        if (properties.Jobs.Deliveries)
        {
            if (GetObjectsOfType(78) + GetObjectsOfType(108) == 0)
                LogError(count++, $"Deliveries: No unloading truck!");
            if (GetObjectsOfType(93) == 0) LogError(count++, $"Deliveries: No blue container!");
            if (GetObjectsOfType(92) == 0) LogError(count++, $"Deliveries: No red container!");
            if (GetZoneOfType("Deliveries")) LogError(count++, $"Deliveries zone is missing!");
        }

        if (properties.Jobs.Mailman)
        {
            if (GetObjectsOfType(82) == 0) LogError(count++, $"Mailman: No mailing cabinet!");
        }

        if (properties.Jobs.Metalshop)
        {
            if (GetObjectsOfType(45) == 0) LogError(count++, $"Metalshop: No licence press!");
            if (GetObjectsOfType(43) == 0) LogError(count++, $"Metalshop: No metal plates container!");
            if (GetObjectsOfType(44) == 0) LogError(count++, $"Metalshop: No licence plates container!");
            if (GetZoneOfType("Metalshop")) LogError(count++, $"Metalshop zone is missing!");
        }
        
        if (GetZoneOfType("SHU")) LogError(count++, $"SHU zone is missing!");
        if (GetZoneOfType("YourCell")) LogError(count++, $"YourCell zone is missing!");
        if (GetZoneOfType("Cells1")) LogError(count++, $"Cells1 zone is missing!");
        if (GetZoneOfType("Shower")) LogError(count++, $"Shower zone is missing!");
        if (GetZoneOfType("Gym")) LogError(count++, $"Gym zone is missing!");
        if (GetZoneOfType("Rollcall")) LogError(count++, $"Rollcall zone is missing!");
        if (GetZoneOfType("Canteen")) LogError(count++, $"Canteen zone is missing!");
    }

    private int GetActiveJobs(Properties properties)
    {
        return typeof(Jobs)
            .GetFields(BindingFlags.Public | BindingFlags.Instance)
            .Where(f => f.FieldType == typeof(bool))
            .Count(f => (bool)f.GetValue(properties.Jobs));
    }

    public void GetAllObjects()
    {
        objects.Clear();
        zones.Clear();

        foreach (GameObject layer in ObjectEditor.instance.ObjectLayers)
        {
            foreach (Transform child in layer.transform)
            {
                objects.Add(child.gameObject);
            }
        }
        
        foreach (Transform child in ZoneEditor.instance.ZoneParent.transform)
        {
            zones.Add(child.gameObject);
        }
    }

    public int GetObjectsOfType(int id)
    {
        var list = objects
            .Where(obj => obj.name.Contains($"[{id}]"))
            .ToList();

        return list.Count;
    }
    
    public bool GetZoneOfType(string zoneName)
    {
        var list = zones
            .Where(obj => obj.name.Contains($"{zoneName}"))
            .ToList();

        return list.Count > 0 ? !true : !false;
    }

    private object GetAllChildrenRecursive(Transform transform1)
    {
        throw new NotImplementedException();
    }

    public void LogError(int count, string error)
    {
        var message = $"{count}. {error}";
        errors.Add(message);
    }
}