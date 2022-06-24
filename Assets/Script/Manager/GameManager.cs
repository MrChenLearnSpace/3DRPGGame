using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public CharacterStates playerStates;
    List<IEndGameObserver> endGameObservers = new List<IEndGameObserver>(); 
    // Start is called before the first frame update
    public void RigisterPlayer(CharacterStates player) {
        playerStates = player;
    }
    public void AddObserver(IEndGameObserver observer) {
        endGameObservers.Add(observer);
    }
    public void RemoveObserver(IEndGameObserver observer) {
        if(endGameObservers.Contains(observer))
            endGameObservers.Remove(observer);
    }
    public void NotifyObserver() {
        for(int i=0;i<endGameObservers.Count;i++) {
            endGameObservers[i].EndNotify();
        }
    }
}
