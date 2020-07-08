using System;
using UnityEngine;
using BattleMath;

public class BattleManager : MonoBehaviour
{
    private Boolean startFight = false;
    private BattleModel preActModel;
    private BattleModel postActModel;
    private ModelMessageWrapper messageWrapper;
    private FrameSyncServices frameSyncServices;
    private MacrophagBehaviour player;
    // Start is called before the first frame update
    void Start()
    {
        preActModel = new BattleModel();
        postActModel = new BattleModel();
        messageWrapper = new ModelMessageWrapper(postActModel);
        preActModel.addPlayer(0, 0);
        postActModel.addPlayer(0, 0);
        player = GameObject.Find("macrophages").GetComponent<MacrophagBehaviour>();
        frameSyncServices = FrameSyncServices.Inst;
    }

    // Update is called once per frame
    void Update()
    {
        checkInput();
    }

    void checkInput()
    {
        if (Input.anyKeyDown) {
            foreach (KeyCode keyCode in Enum.GetValues (typeof (KeyCode))) {
                if (Input.GetKeyDown (keyCode)) {
                    switch(keyCode) {
                        case KeyCode.W: case KeyCode.A: case KeyCode.S: case KeyCode.D: 
                            onMoveBtnPressed(keyCode); 
                            break;
                    }
                }
            }
        } 
    }

    void onMoveBtnPressed(KeyCode key) 
    {
        if(!startFight) {
            frameSyncServices.StartFight();
            startFight = true;
        }
        PlayerOperation operation;
        switch(key) {
            case KeyCode.W: operation = preActModel.move(0, new vector2(0, 0.5m)); break;
            case KeyCode.S: operation = preActModel.move(0, new vector2(0, -0.5m)); break;
            case KeyCode.A: operation = preActModel.move(0, new vector2(-0.5m, 0)); break;
            case KeyCode.D: operation = preActModel.move(0, new vector2(0.5m, 0)); break;
            default: return;
        }
        renderByModel(preActModel);
        frameSyncServices.DoInput(operation.ToString());
    }

    void renderByModel(BattleModel model)
    {
        vector2 player_pos = model.getPlayerPosition(0);
        this.player.setPosition(player_pos.toVec2());
    }
}
