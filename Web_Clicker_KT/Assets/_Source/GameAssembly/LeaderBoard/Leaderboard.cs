using FireBase;
using TMPro;
using UnityEngine;

namespace LeaderBoard
{
    public class Leaderboard : MonoBehaviour
    {
        [SerializeField] private FBAuthManager fbAuthManager;
        [SerializeField] private TMP_Text firstPlace;
        [SerializeField] private TMP_Text secondPlace;
        [SerializeField] private TMP_Text thirdPlace;

        public void DrawLeaderboard()
        { 
            UpdateLeaderboard();
        }

        private void UpdateLeaderboard()
        {
            var task = fbAuthManager.UpdateLeaderBoard();

            firstPlace.text = task[0].UserName + " - " + task[0].Clicks.ToString();
            secondPlace.text = task[1].UserName + " - " + task[1].Clicks.ToString();
            thirdPlace.text = task[2].UserName + " - " + task[2].Clicks.ToString();
        }
    }
}