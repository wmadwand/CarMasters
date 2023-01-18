using UnityEngine;
using UnityEditor;
public class ChangeBike : EditorWindow
{
    public int index = 0;
    public string[] BodyKitOptions = new string[] { "Body 1", "Body 2", "Body 3", "Body 4"};
    [MenuItem("Change Bike/Select Bike")]
    public static void GenerateDriftCar()
    {
        GetWindow<ChangeBike>("Select Bike");
    }
    void OnGUI()
    {
        GUI.color = Color.white;
        GUILayout.Label("Select a bike to replace Player's bike\n\n", EditorStyles.boldLabel);
        GUILayout.Space(125);
        Texture tex1 = (Texture)AssetDatabase.LoadAssetAtPath("Assets/Simple Motorcycle Physics/Editor/Editor Icons/1.png", typeof(Texture));
        Texture tex2 = (Texture)AssetDatabase.LoadAssetAtPath("Assets/Simple Motorcycle Physics/Editor/Editor Icons/2.png", typeof(Texture));
        Texture tex3 = (Texture)AssetDatabase.LoadAssetAtPath("Assets/Simple Motorcycle Physics/Editor/Editor Icons/3.png", typeof(Texture));
        Texture tex4 = (Texture)AssetDatabase.LoadAssetAtPath("Assets/Simple Motorcycle Physics/Editor/Editor Icons/4.png", typeof(Texture));

        EditorGUI.LabelField(new Rect(25, 40, 100, 15), "Body 1:");
        EditorGUI.DrawPreviewTexture(new Rect(25, 60, 100, 100), tex1);

        EditorGUI.LabelField(new Rect(25 + 120, 40, 100, 15), "Body 2:");
        EditorGUI.DrawPreviewTexture(new Rect(25 + 120, 60, 100, 100), tex2);

        EditorGUI.LabelField(new Rect(25 + 240, 40, 100, 15), "Body 3:");
        EditorGUI.DrawPreviewTexture(new Rect(25 + 240, 60, 100, 100), tex3);

        EditorGUI.LabelField(new Rect(25 + 240, 40, 100, 15), "Body 4:");
        EditorGUI.DrawPreviewTexture(new Rect(25 + 360, 60, 100, 100), tex4);

        index = EditorGUILayout.Popup(index, BodyKitOptions);

        if (GUILayout.Button("Switch Bike"))
        {
            switch (index)
            {
                case 0: Replace("Motorcycle");
                break;
                case 1: Replace("Motorcycle 2");
                break;
                case 2: Replace("Motorcycle 3");
                break;
                case 3: Replace("Motorcycle 4");
                break;
            }

    }
}
void Replace(string s)
{
    GameObject player;
    player = GameObject.FindGameObjectWithTag("Player");
    var m1 = player.transform.Find("Motorcycle"); 
    m1.gameObject.SetActive(false);
    m1 = player.transform.Find("Motorcycle 2");
    m1.gameObject.SetActive(false);
    m1 = player.transform.Find("Motorcycle 3");
    m1.gameObject.SetActive(false);
    m1 = player.transform.Find("Motorcycle 4");
    m1.gameObject.SetActive(false);
    m1 = player.transform.Find(s);
    m1.gameObject.SetActive(true);
    player.GetComponent<MotorbikeController>().handles = m1.transform.Find("Handle System").gameObject;
    player.GetComponent<MotorbikeController>().RearMudGuard = m1.transform.Find("Rear MudGuard System").gameObject;
    EditorUtility.SetDirty(player.gameObject);
}
}
