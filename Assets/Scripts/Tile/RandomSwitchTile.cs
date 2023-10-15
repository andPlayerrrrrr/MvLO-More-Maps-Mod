using UnityEngine;
using Photon.Pun;
using NSMB.Utils;

[CreateAssetMenu(fileName = "RandomSwitchTile", menuName = "ScriptableObjects/Tiles/Switchs/RandomSwitchTile", order = 0)]
public class RandomSwitchTile : InteractableTile {
    [ColorUsage(false)]
    public Color particleColor;
    public bool breakableBySmallMario = false, breakableByLargeMario = false, breakableByGiantMario = false, breakableByShells = false, breakableByBombs = false, bumpIfNotBroken = true, bumpIfBroken = false,onlyOne = false, lockmode = false, nextbrake = true;
    private bool off = true;
    public int[] otherSpawnPointX = new int[1];
    public int[] otherSpawnPointY = new int[1];
    public int[] repeatX = new int[1];
    public int[] repeatY = new int[1];
    public string[] resultTile = new string[1];
    public string Path = "Switchs/";
    public bool breakSound = false, breakParticle = false;
    public Color[] breakTileColor = new Color[1];
    public int[] Rates;
    private int a = 0;
    protected bool BreakBlockCheck(MonoBehaviour interacter, InteractionDirection direction, Vector3 worldLocation) {
        bool doBump = false, doBreak = false, giantBreak = false;
        if (interacter is PlayerController pl) {
            if (pl.state <= Enums.PowerupState.Small && !pl.drill) {
                doBreak = breakableBySmallMario;
                doBump = true;
            } else if (pl.state == Enums.PowerupState.MegaMushroom) {
                doBreak = breakableByGiantMario;
                giantBreak = true;
                doBump = false;
            } else if (pl.state >= Enums.PowerupState.Mushroom || pl.drill) {
                doBreak = breakableByLargeMario;
                doBump = true;
            }

        } else if (interacter is SpinyWalk) {
            doBump = true;
            doBreak = breakableByShells;
        } else if (interacter is KoopaWalk) {
            doBump = true;
            doBreak = breakableByShells;
        } else if (interacter is BobombWalk) {
            doBump = false;
            doBreak = breakableByBombs;
        }
        if (doBump && doBreak && bumpIfBroken)
            Bump(interacter, direction, worldLocation);
        if (doBump && !doBreak && bumpIfNotBroken)
        BumpWithAnimation(interacter, direction, worldLocation);
        SetEvent(interacter, worldLocation);
        if (doBreak)
            Break(interacter, worldLocation, giantBreak ? Enums.Sounds.Powerup_MegaMushroom_Break_Block : Enums.Sounds.World_Block_Break);
        return doBreak;
    }
        public void Break(MonoBehaviour interacter, Vector3 worldLocation, Enums.Sounds sound) {
        Vector3Int tileLocation = Utils.WorldToTilemapPosition(worldLocation);

        //Tilemap
        object[] parametersTile = new object[] { tileLocation.x, tileLocation.y, null };
        GameManager.Instance.SendAndExecuteEvent(Enums.NetEventIds.SetTile, parametersTile, ExitGames.Client.Photon.SendOptions.SendReliable);

        object[] parametersParticle = new object[]{ tileLocation.x, tileLocation.y, "BrickBreak", new Vector3(particleColor.r, particleColor.g, particleColor.b) };
        GameManager.Instance.SendAndExecuteEvent(Enums.NetEventIds.SpawnParticle, parametersParticle, ExitGames.Client.Photon.SendOptions.SendUnreliable);

        if (interacter is MonoBehaviourPun pun)
            pun.photonView.RPC("PlaySound", RpcTarget.All, sound);
    }
    public void BumpWithAnimation(MonoBehaviour interacter, InteractionDirection direction, Vector3 worldLocation) {
        Bump(interacter, direction, worldLocation);
        Vector3Int tileLocation = Utils.WorldToTilemapPosition(worldLocation);

        //Bump
        object[] parametersBump = new object[]{tileLocation.x, tileLocation.y, direction == InteractionDirection.Down, "SpecialTiles/" + Path + name, ""};
        GameManager.Instance.SendAndExecuteEvent(Enums.NetEventIds.BumpTile, parametersBump, ExitGames.Client.Photon.SendOptions.SendReliable);
    }
    public void SetEvent(MonoBehaviour interacter, Vector3 worldLocation)
    {
        Vector3Int tileLocation = Utils.WorldToTilemapPosition(worldLocation);
        int next = a;
        int rand = 0;
        int sumWeight = 0;
        for (int i = 0; i < Rates.Length; i++)
        {
            sumWeight += Rates[i];
        }
        for (int i = 0; i < Rates.Length; i++)
        {
            if(Random.Range(0,sumWeight) < Rates[i])
            {
                rand = i;
                a = i;
                break;
            }
            else
            {
                sumWeight -= Rates[i];
            }
        }
        //Tilemap
        if (off == true && lockmode == false && onlyOne == false)
        {
            if(nextbrake == true)
            {
                for (int i = 0; i < repeatX[next]; i++)
                {
                    object[] parametersTile = new object[] { otherSpawnPointX[next] + i, otherSpawnPointY[next], null };
                    GameManager.Instance.SendAndExecuteEvent(Enums.NetEventIds.SetTile, parametersTile, ExitGames.Client.Photon.SendOptions.SendReliable);
                }
                for (int k = 0; k < repeatY[next]; k++)
                {
                    object[] parametersTile = new object[] { otherSpawnPointX[next], otherSpawnPointY[next] + k, null };
                    GameManager.Instance.SendAndExecuteEvent(Enums.NetEventIds.SetTile, parametersTile, ExitGames.Client.Photon.SendOptions.SendReliable);
                }
            }
            else if (nextbrake == false)
            {
                for (int i = 0; i < repeatX[rand]; i++)
                {
                    object[] parametersTile = new object[] { otherSpawnPointX[rand] + i, otherSpawnPointY[rand], null };
                    GameManager.Instance.SendAndExecuteEvent(Enums.NetEventIds.SetTile, parametersTile, ExitGames.Client.Photon.SendOptions.SendReliable);
                }
                for (int k = 0; k < repeatY[rand]; k++)
                {
                    object[] parametersTile = new object[] { otherSpawnPointX[rand], otherSpawnPointY[rand] + k, null };
                    GameManager.Instance.SendAndExecuteEvent(Enums.NetEventIds.SetTile, parametersTile, ExitGames.Client.Photon.SendOptions.SendReliable);
                }
            }
            off = false;
        }
        else if(off == false && lockmode == false && onlyOne == false)
        {
                for (int j = 0; j < repeatX[rand]; j++)
                {
                    object[] parametersTile = new object[] { otherSpawnPointX[rand] + j, otherSpawnPointY[rand], resultTile[rand] };
                    GameManager.Instance.SendAndExecuteEvent(Enums.NetEventIds.SetTile, parametersTile, ExitGames.Client.Photon.SendOptions.SendReliable);
                }
            for (int l = 0; l < repeatY[rand]; l++)
            {
                object[] parametersTile = new object[] { otherSpawnPointX[rand], otherSpawnPointY[rand] + l, resultTile[rand] };
                GameManager.Instance.SendAndExecuteEvent(Enums.NetEventIds.SetTile, parametersTile, ExitGames.Client.Photon.SendOptions.SendReliable);
            }
            off = true;
        }
        else if(lockmode == true && onlyOne == false)
        {
                for (int q = 0; q < repeatX[rand]; q++)
                {
                    object[] parametersTile = new object[] { otherSpawnPointX[rand] + q, otherSpawnPointY[rand], resultTile[rand] };
                    GameManager.Instance.SendAndExecuteEvent(Enums.NetEventIds.SetTile, parametersTile, ExitGames.Client.Photon.SendOptions.SendReliable);
                }
                for (int w = 0; w < repeatY[rand]; w++)
                {
                    object[] parametersTile = new object[] { otherSpawnPointX[rand], otherSpawnPointY[rand] + w, resultTile[rand] };
                    GameManager.Instance.SendAndExecuteEvent(Enums.NetEventIds.SetTile, parametersTile, ExitGames.Client.Photon.SendOptions.SendReliable);
                }
        }
        else if(lockmode == false && onlyOne == true)
        {
                for (int a = 0; a < repeatX[rand]; a++)
                {
                    object[] parametersTile = new object[] { otherSpawnPointX[rand] + a, otherSpawnPointY[rand], resultTile[rand] };
                    GameManager.Instance.SendAndExecuteEvent(Enums.NetEventIds.SetTile, parametersTile, ExitGames.Client.Photon.SendOptions.SendReliable);
                }
                for (int b = 0; b < repeatY[rand]; b++)
                {
                    object[] parametersTile = new object[] { otherSpawnPointX[rand], otherSpawnPointY[rand] + b, resultTile[rand] };
                    GameManager.Instance.SendAndExecuteEvent(Enums.NetEventIds.SetTile, parametersTile, ExitGames.Client.Photon.SendOptions.SendReliable);
                }
            Break(interacter, worldLocation, Enums.Sounds.World_Block_Break);
        }
        if (breakSound == true) sound(interacter, worldLocation, breakParticle, rand, next);
    }
    public void sound(MonoBehaviour interacter, Vector3 worldLocation, bool breakParticle, int rand, int next)
    {
        Vector3Int tileLocation = Utils.WorldToTilemapPosition(worldLocation);
        if(breakParticle == true)
        {
            if(nextbrake == false)
            {
                for (int i = 0; i < repeatX[rand]; i++)
                {
                    object[] parametersParticle = new object[] { otherSpawnPointX[rand] + i, otherSpawnPointY[rand], "BrickBreak", new Vector3(breakTileColor[rand].r, breakTileColor[rand].g, breakTileColor[rand].b) };
                    GameManager.Instance.SendAndExecuteEvent(Enums.NetEventIds.SpawnParticle, parametersParticle, ExitGames.Client.Photon.SendOptions.SendUnreliable);
                }
                for (int k = 0; k < repeatY[rand]; k++)
                {
                    object[] parametersParticle = new object[] { otherSpawnPointX[rand], otherSpawnPointY[rand] + k, "BrickBreak", new Vector3(breakTileColor[rand].r, breakTileColor[rand].g, breakTileColor[rand].b) };
                    GameManager.Instance.SendAndExecuteEvent(Enums.NetEventIds.SpawnParticle, parametersParticle, ExitGames.Client.Photon.SendOptions.SendUnreliable);
                }
            }
            else if(nextbrake == true)
            {
                for (int i = 0; i < repeatX[next]; i++)
                {
                    object[] parametersParticle = new object[] { otherSpawnPointX[next] + i, otherSpawnPointY[next], "BrickBreak", new Vector3(breakTileColor[next].r, breakTileColor[next].g, breakTileColor[next].b) };
                    GameManager.Instance.SendAndExecuteEvent(Enums.NetEventIds.SpawnParticle, parametersParticle, ExitGames.Client.Photon.SendOptions.SendUnreliable);
                }
                for (int k = 0; k < repeatY[next]; k++)
                {
                    object[] parametersParticle = new object[] { otherSpawnPointX[next], otherSpawnPointY[next] + k, "BrickBreak", new Vector3(breakTileColor[next].r, breakTileColor[next].g, breakTileColor[next].b) };
                    GameManager.Instance.SendAndExecuteEvent(Enums.NetEventIds.SpawnParticle, parametersParticle, ExitGames.Client.Photon.SendOptions.SendUnreliable);
                }
            }
        }
        if (interacter is MonoBehaviourPun pun)
            pun.photonView.RPC("PlaySound", RpcTarget.All, Enums.Sounds.World_Block_Break);
    }
    public override bool Interact(MonoBehaviour interacter, InteractionDirection direction, Vector3 worldLocation) {
        //Breaking block check.
        return BreakBlockCheck(interacter, direction, worldLocation);
    }
}
