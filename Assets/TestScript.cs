using UnityEngine;

public class TestScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        BattleModel model = new BattleModel();
        ModelMessageWrapper wrapper = new ModelMessageWrapper(model);
        wrapper.doTest();
        NetLogic.Inst.Init(0);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
