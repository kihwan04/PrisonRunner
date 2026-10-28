using System;
using System.Collections.Generic;
using System.Globalization;
using Muhanok.Domain;
using UnityEngine;

namespace Muhanok.Presentation
{
    // Distance ranking with all-time and today tabs. After a run it also shows that run's summary and highlights its row.
    internal sealed class LeaderboardScreen
    {
        static readonly CultureInfo Num = CultureInfo.InvariantCulture;
        // card slots left to right: 2nd, 1st, 3rd
        static readonly int[] SlotRank = { 1, 0, 2 };
        static readonly float[] SlotHeight = { 138, 158, 128 };

        readonly OverlaySkin skin;
        bool todayOnly, resultMode;
        RankingEntry mine;
        List<RankingEntry> list = new List<RankingEntry>();
        string summary = "";

        readonly GuestNameRow guestName;
        public LeaderboardScreen(OverlaySkin overlaySkin) { skin = overlaySkin; guestName = new GuestNameRow(overlaySkin); }
        public bool AsksGuestName => resultMode && guestName.Open;

        // From the menu: developer (exhibition) mode starts on today's tab.
        public void OpenBoard(IHudData data)
        {
            resultMode = false; mine = null; guestName.Skip();
            todayOnly = data != null && data.DeveloperMode;
            Reload(data);
        }

        // After a run: today's tab first so players see where they stand among today's runs.
        public void OpenResult(IHudData data, RunSession session)
        {
            resultMode = true; todayOnly = true; mine = data?.LastEntry;
            var parts = new List<string>();
            if (data != null && data.LastTodayRank > 0) parts.Add("오늘 " + data.LastTodayRank + "위");
            if (data != null && data.LastRank > 0) parts.Add("전체 " + data.LastRank + "위");
            parts.Add("코인 +" + session.Score.Coins);
            if (PlayStyleInfo.UsesCamera(data?.Player?.Style ?? PlayStyle.Keyboard) && session.Workout.WeightKg.HasValue) parts.Add(session.Workout.EstimatedKcal.ToString("F1", Num) + " kcal");
            summary = string.Join("  ·  ", parts);
            guestName.Show(data);
            Reload(data);
        }

        void Reload(IHudData data)
        {
            list = data?.Board != null ? data.Board.View(todayOnly, DateTime.Now) : new List<RankingEntry>();
        }

        static string Dist(float m) { return Mathf.FloorToInt(m).ToString("N0", Num) + "m"; }

        // primaryLabel/secondaryLabel: result buttons (다시 하기 / 메인으로 or 다음 플레이어); the menu board only shows a close button.
        public void Draw(IHudData data, RunSession session, Action primary, string primaryLabel, Action secondary, string secondaryLabel)
        {
            var oldMatrix = GUI.matrix;
            GUI.matrix = Matrix4x4.identity;
            skin.Fill(new Rect(0, 0, Screen.width, Screen.height), Palette.A(Palette.Ground, 0.9f));
            GUI.matrix = oldMatrix;

            float x = 230, w = 820;
            if (resultMode)
            {
                bool newBest = data != null && data.LastNewBest;
                skin.Text(new Rect(x, 30, 500, 52), Dist(session.Score.Distance), 44, Palette.Cream, TextAnchor.MiddleLeft);
                if (newBest) skin.Text(new Rect(x + 230, 30, 260, 52), "최고 기록 갱신!", 18, Palette.MeNote, TextAnchor.MiddleLeft, false, true);
                skin.Text(new Rect(x, 78, 600, 22), summary, 15, Palette.Sand, TextAnchor.MiddleLeft, false, true);
            }
            else skin.Text(new Rect(x, 36, 300, 48), "랭킹", 44, Palette.Cream, TextAnchor.MiddleLeft);
            DrawTabs(data, new Rect(x + w - 176, 38, 176, 44));
            DrawTop3(x, w);
            DrawRows(x, w);
            DrawMine(data, x, w);
            if(resultMode)DrawWorkout(session, data);
            if(resultMode)guestName.Draw(data, new Rect(x + 110, 290, w - 220, 180));

            if (secondary == null)
            {
                if (skin.Button(new Rect(540, 606, 200, 56), primaryLabel, Palette.A(Palette.Ground, 0.9f), Palette.Wood, Palette.Cream, 22)) primary?.Invoke();
                return;
            }
            if (skin.Button(new Rect(434, 606, 200, 56), primaryLabel, Palette.Gold, Color.clear, Palette.Outline, 22)) primary?.Invoke();
            if (skin.Button(new Rect(646, 606, 200, 56), secondaryLabel, Palette.A(Palette.Ground, 0.9f), Palette.Wood, Palette.Cream, 22)) secondary.Invoke();
        }

        void DrawWorkout(RunSession session, IHudData data)
        {
            skin.BorderBox(new Rect(24,140,190,380),Palette.A(Palette.Panel,.95f),Palette.Wood,2);
            skin.Text(new Rect(38,152,164,30),"운동 기록",22,Palette.Gold,TextAnchor.MiddleLeft);
            skin.Text(new Rect(38,190,164,26),"최고 콤보 "+session.BestDodgeStreak,16,Palette.Cream,TextAnchor.MiddleLeft,false,true);
            if (!PlayStyleInfo.UsesCamera(data?.Player?.Style ?? PlayStyle.Keyboard) || !session.Workout.WeightKg.HasValue)
            {
                skin.Text(new Rect(38,235,164,30),"칼로리 계산 제외",16,Palette.Sand,TextAnchor.MiddleLeft,false,true);
                return;
            }
            skin.Text(new Rect(38,220,164,26),"목표 "+session.Workout.GoalKcal.ToString("F0")+" kcal",14,Palette.Sand,TextAnchor.MiddleLeft,false,true);
            for(int i=0;i<5;i++)
            {
                skin.Text(new Rect(38,258+i*46,164,22),WorkoutEstimate.ActivityNames[i],12,Palette.Sand,TextAnchor.MiddleLeft,false,true);
                skin.Text(new Rect(38,278+i*46,164,22),session.Workout.KcalFor(i).ToString("F2")+" kcal",15,Palette.Cream,TextAnchor.MiddleLeft,false,true);
            }
        }

        void DrawTabs(IHudData data, Rect r)
        {
            skin.Box(r, Palette.Panel);
            var all = new Rect(r.x + 4, r.y + 4, 84, 36);
            var today = new Rect(r.x + 88, r.y + 4, 84, 36);
            if (Tab(all, "전체", !todayOnly)) { todayOnly = false; Reload(data); }
            if (Tab(today, "오늘", todayOnly)) { todayOnly = true; Reload(data); }
        }

        bool Tab(Rect r, string label, bool on)
        {
            if (on) skin.Box(r, Palette.Gold);
            skin.Text(r, label, 16, on ? Palette.Outline : Palette.Sand, TextAnchor.MiddleCenter, false, true);
            return GUI.Button(r, GUIContent.none, GUIStyle.none)||GesturePointer.Activate(r,label);
        }

        void DrawTop3(float x, float w)
        {
            float cardW = (w - 32) / 3f, bottom = 102 + 158;
            Color[] rankColor = { Palette.Gold, Palette.Silver, Palette.Bronze };
            for (int s = 0; s < 3; s++)
            {
                int rank = SlotRank[s];
                if (rank >= list.Count) continue;
                var r = new Rect(x + s * (cardW + 16), bottom - SlotHeight[s], cardW, SlotHeight[s]);
                skin.BorderBox(r, Palette.A(Palette.Panel, 0.95f), rankColor[rank], 2);
                float cy = r.center.y;
                skin.Text(new Rect(r.x, cy - 54, r.width, 34), (rank + 1).ToString(), 30, rankColor[rank], TextAnchor.MiddleCenter);
                skin.Text(new Rect(r.x, cy - 16, r.width, 24), list[rank].Name, 17, Palette.Cream, TextAnchor.MiddleCenter, false, true);
                skin.Text(new Rect(r.x, cy + 12, r.width, 34), Dist(list[rank].Distance), 28, Palette.Cream, TextAnchor.MiddleCenter);
            }
            if (list.Count == 0)
                skin.Text(new Rect(x, 160, w, 60), todayOnly ? "오늘은 아직 기록이 없어요. 첫 기록을 남겨 보세요!" : "아직 기록이 없어요. 첫 기록을 남겨 보세요!", 18, Palette.Sand, TextAnchor.MiddleCenter, false, true);
        }

        void DrawRows(float x, float w)
        {
            for (int i = 0; i < 5; i++)
            {
                int rank = i + 3;
                if (rank >= list.Count) break;
                var r = new Rect(x, 278 + i * 48, w, 42);
                skin.BorderBox(r, Palette.A(Palette.Panel, 0.92f), Palette.WoodDim, 2);
                Columns(r, rank + 1, list[rank].Name, Dist(list[rank].Distance), 18, 20, Palette.Muted, Palette.Cream);
            }
        }

        void DrawMine(IHudData data, float x, float w)
        {
            int me = mine != null ? list.IndexOf(mine) : -1;
            if (me < 0) return;
            var row = new Rect(x, 530, w, 54);
            float pulse = 0.5f + 0.5f * Mathf.Sin(Time.unscaledTime * 4f);
            skin.BorderBox(row, Palette.MeFill, Color.Lerp(Palette.Gold, Palette.Cream, pulse * 0.35f), 2);
            // Developer mode has a name per run; otherwise every row is this PC's player, so the latest run reads "나".
            string name = data != null && data.DeveloperMode ? mine.Name : "나";
            Columns(row, me + 1, name, Dist(mine.Distance), 20, 24, Palette.Gold, Palette.Gold);
        }

        void Columns(Rect r, int rank, string name, string dist, int rankSize, int distSize, Color rankColor, Color distColor)
        {
            skin.Text(new Rect(r.x + 20, r.y, 60, r.height), rank.ToString(), rankSize, rankColor, TextAnchor.MiddleLeft);
            skin.Text(new Rect(r.x + 84, r.y, r.width - 260, r.height), name, 16, Palette.Cream, TextAnchor.MiddleLeft, false, true);
            skin.Text(new Rect(r.xMax - 180, r.y, 160, r.height), dist, distSize, distColor, TextAnchor.MiddleRight);
        }
    }
}
