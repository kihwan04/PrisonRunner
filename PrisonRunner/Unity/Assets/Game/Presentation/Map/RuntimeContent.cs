using System;
using System.Collections.Generic;
using Muhanok.Domain;
using UnityEngine;

namespace Muhanok.Presentation.Map
{
    public interface IRunnerAnimated { void Preview(float runnerZ); }

    public sealed class ItemPickup : MonoBehaviour
    {
        public ItemKind Kind;
        public bool Taken { get; private set; }
        public void ResetPickup() { Taken = false; }
        public void Evaluate(RunSession run, float previousZ)
        {
            if (Taken || !gameObject.activeSelf || run.Flow.State != GameFlowState.Running || run.Paused) return;
            float z = transform.position.z;
            if (previousZ > z || run.Runner.Z < z) return;
            RouteSurface.Sample(run.Runner.Z, out float x, out _);
            if (Mathf.Abs(run.Runner.WorldLaneOffset + x - transform.position.x) > 1f) return;
            Taken = true; run.Items.Activate(Kind); gameObject.SetActive(false);
        }
        private void Update() { transform.Rotate(0, Time.deltaTime * 120, 0, Space.Self); }
    }

    // Both animals lower a visible low obstacle across the path: jump after the warning.
    public sealed class ZooHazard : MonoBehaviour, IRunnerAnimated
    {
        public bool Giraffe;
        public Transform Moving;
        public ObstacleController[] Hazards;
        public void Preview(float runnerZ)
        {
            if (Moving == null) return;
            float t = Mathf.InverseLerp(32f, 14f, transform.position.z - runnerZ);
            if (Giraffe) Moving.localRotation = Quaternion.Euler(0, 0, Mathf.Lerp(8f, 90f, Mathf.SmoothStep(0, 1, t)));
            else Moving.localRotation = Quaternion.Euler(0, Mathf.Sin(Time.time * 3f) * 10f * (1 - t), Mathf.Lerp(-55f, 0f, Mathf.SmoothStep(0, 1, t)));
        }
    }
    public sealed class ThemedLaneBlock : MonoBehaviour
    {
        public int Theme;
        public ObstacleController Hazard;
    }

    // Places items and zoo animals into walking chunks at runtime, from pools, without touching the authored prefabs.
    public sealed class RuntimeContent : ChunkSpawner.IChunkDecorator
    {
        private readonly Transform root;
        private readonly Func<List<ItemKind>> availableItems;
        private readonly float itemChance;
        private readonly Dictionary<ItemKind, Stack<ItemPickup>> itemPool = new Dictionary<ItemKind, Stack<ItemPickup>>();
        private readonly Stack<ZooHazard> giraffes = new Stack<ZooHazard>(), elephants = new Stack<ZooHazard>();
        private readonly Stack<FloorItemController> bananas = new Stack<FloorItemController>();
        private readonly Stack<ThemedLaneBlock>[] blockers = { new Stack<ThemedLaneBlock>(), new Stack<ThemedLaneBlock>(), new Stack<ThemedLaneBlock>() };
        private readonly Dictionary<MuhanokMapChunk, List<Component>> placed = new Dictionary<MuhanokMapChunk, List<Component>>();
        private readonly Stack<List<Component>> placementLists = new Stack<List<Component>>();
        private static readonly Dictionary<Color, Material> materials = new Dictionary<Color, Material>();
        public RuntimeContent(Transform parent, Func<List<ItemKind>> items, float chance)
        {
            root = new GameObject("RuntimeContent").transform; root.SetParent(parent, false);
            availableItems = items; itemChance = chance;
            RailForkView.PrewarmMeshes();
            foreach(var chunk in parent.GetComponentsInChildren<MuhanokMapChunk>(true))if(chunk.RouteType==1||chunk.RouteType==2)RailForkView.PrewarmInstance(chunk);
            for (int i = 0; i < 4; i++) { var g = BuildGiraffe(); g.gameObject.SetActive(false); giraffes.Push(g); var e = BuildElephant(); e.gameObject.SetActive(false); elephants.Push(e); }
            for(int i=0;i<28;i++){var peel=BuildBanana();peel.gameObject.SetActive(false);bananas.Push(peel);}
            for(int theme=0;theme<3;theme++)for(int i=0;i<10;i++){var block=BuildBlock(theme);block.gameObject.SetActive(false);blockers[theme].Push(block);}
        }

        public void Decorate(MuhanokMapChunk chunk, int sequence, int type, System.Random random)
        {
            bool cart = type == 1 || type == 2;
            var list = placementLists.Count > 0 ? placementLists.Pop() : new List<Component>(3); placed[chunk] = list;
            bool expanded=RouteSurface.ActivePlan!=null&&RouteSurface.ActivePlan.Extended;
            bool fork=expanded&&RouteSurface.ActivePlan.IsForkTrackChunk(sequence);
            if(expanded)GroundSurfaceOwner.Prepare(chunk,fork||type==3);
            if(expanded&&!fork)
            {
                if(type==3){list.Add(RouteTransitionView.Attach(chunk));return;}
                list.Add(RouteEnvelopeView.Attach(chunk));
            }
            if(sequence==0)return;
            if (cart)
            {
                if (RouteSurface.ActivePlan != null && RouteSurface.ActivePlan.IsForkTrackChunk(sequence))
                { var junction = RailForkView.Attach(chunk, sequence); list.Add(junction); }
                return;
            }
            float start = chunk.Entry.position.z;
            bool walking=type==0||type==4||type==5||type==12;
            int safeLane=random.Next(3)-1;
            bool blocked=expanded&&walking&&random.NextDouble()<(type==12?.52:.72);
            if(expanded&&walking&&chunk.FloorItems!=null)foreach(var h in chunk.FloorItems)h.gameObject.SetActive(false);
            if(blocked)
            {
                foreach(var h in chunk.Obstacles)h.gameObject.SetActive(false);
                foreach(var h in chunk.ExerciseObstacles)h.gameObject.SetActive(false);
                int theme=type==12?2:type==0?0:1;
                bool doubleBlock=random.NextDouble()<.85;
                int firstBlocked=(safeLane+2)%3-1;
                for(int lane=-1;lane<=1;lane++)
                {
                    if(lane==safeLane||(!doubleBlock&&lane!=firstBlocked))continue;
                    var block=blockers[theme].Count>0?blockers[theme].Pop():BuildBlock(theme);
                    block.transform.position=LanePoint(lane,start+12,0);block.transform.SetParent(chunk.transform,true);
                    block.Hazard.ResetPass();block.gameObject.SetActive(true);
                    chunk.DynamicObstacles.Add(block.Hazard);list.Add(block);
                }
                if(chunk.Coins!=null)foreach(var coin in chunk.Coins)
                {
                    var p=coin.transform.position;p.x=LanePoint(safeLane,p.z,0).x;coin.transform.position=p;
                }
            }
            // Zoo animals: one per zoo chunk, in a stretch with no authored hazard nearby.
            if (type == 12 && !blocked)
            {
                foreach (var h in chunk.Obstacles) h.gameObject.SetActive(false);
                foreach (var h in chunk.ExerciseObstacles) h.gameObject.SetActive(false);
                if (chunk.FloorItems != null) foreach (var h in chunk.FloorItems) h.gameObject.SetActive(false);
                float z = start + 12f;
                if (!float.IsNaN(z))
                {
                    var animal = sequence % 2 == 0 ? Giraffe(z) : Elephant(z, -1);
                    animal.transform.SetParent(chunk.transform, true);
                    chunk.DynamicObstacles.AddRange(animal.Hazards); chunk.Animated.Add(animal); list.Add(animal);
                }
            }
            if(expanded&&walking)
            {
                // A reserved lane stays clear of all peels and lane blockers in this chunk.
                // Rows are well away from the central animal/blocker, with four metres at chunk seams.
                for(int row=0;row<2;row++)
                {
                    float z=start+(row==0?4:20);
                    if(!Clear(chunk.Obstacles,z,4)||!Clear(chunk.ExerciseObstacles,z,4)||!Clear(chunk.DynamicObstacles,z,4))continue;
                    for(int lane=-1;lane<=1;lane++)if(lane!=safeLane&&random.NextDouble()<.82)
                    {
                        var peel=bananas.Count>0?bananas.Pop():BuildBanana();
                        peel.transform.position=LanePoint(lane,z,.045f);peel.transform.rotation=Quaternion.Euler(0,random.Next(360),0);
                        peel.transform.SetParent(chunk.transform,true);peel.ResetItem();peel.gameObject.SetActive(true);
                        chunk.DynamicFloorItems.Add(peel);list.Add(peel);
                    }
                }
            }
            var kinds = availableItems();
            if (kinds.Count > 0 && random.NextDouble() < itemChance)
            {
                float z = FreeZ(chunk, start, random, 3f);
                if (!float.IsNaN(z))
                {
                    var pickup = Item(kinds[random.Next(kinds.Count)], z, random.Next(3) - 1);
                    pickup.transform.SetParent(chunk.transform, true);
                    chunk.Pickups.Add(pickup); list.Add(pickup);
                }
            }
        }

        // Tutorial: one pooled lane block at a fixed spot of a chunk that was already decorated.
        public ObstacleController PlaceLaneBlock(MuhanokMapChunk chunk, int lane, float z)
        {
            if (!placed.TryGetValue(chunk, out var list)) return null;
            int theme = chunk.RouteType == 12 ? 2 : chunk.RouteType <= 3 ? 0 : 1;
            var block = blockers[theme].Count > 0 ? blockers[theme].Pop() : BuildBlock(theme);
            block.transform.position = LanePoint(lane, z, 0); block.transform.SetParent(chunk.transform, true);
            block.Hazard.ResetPass(); block.gameObject.SetActive(true);
            chunk.DynamicObstacles.Add(block.Hazard); list.Add(block);
            return block.Hazard;
        }

        public void Release(MuhanokMapChunk chunk)
        {
            chunk.GetComponent<GroundSurfaceOwner>()?.Restore();
            chunk.DynamicObstacles.Clear(); chunk.DynamicFloorItems.Clear(); chunk.Pickups.Clear(); chunk.Animated.Clear();
            if (!placed.TryGetValue(chunk, out var list)) return;
            foreach (var item in list)
            {
                if (item is RailForkView fork) { fork.Release(); continue; }
                if(item is RouteEnvelopeView envelope){envelope.Release();continue;}
                if(item is RouteTransitionView connection){connection.Release();continue;}
                item.gameObject.SetActive(false); item.transform.SetParent(root, true);
                if (item is ItemPickup pickup) Pool(pickup.Kind).Push(pickup);
                else if (item is ZooHazard animal) (animal.Giraffe ? giraffes : elephants).Push(animal);
                else if(item is FloorItemController peel)bananas.Push(peel);
                else if(item is ThemedLaneBlock block)blockers[block.Theme].Push(block);
            }
            list.Clear(); placementLists.Push(list); placed.Remove(chunk);
        }

        // A z inside the chunk at least `clearance` metres from every active authored hazard.
        private static float FreeZ(MuhanokMapChunk chunk, float start, System.Random random, float clearance)
        {
            for (int attempt = 0; attempt < 6; attempt++)
            {
                float z = start + 4f + (float)random.NextDouble() * 16f;
                if (Clear(chunk.Obstacles, z, clearance) && Clear(chunk.ExerciseObstacles, z, clearance) && Clear(chunk.DynamicObstacles, z, clearance)
                    && ClearItems(chunk.FloorItems, z, clearance) && ClearGates(chunk, z, clearance)) return z;
            }
            return float.NaN;
        }
        private static bool Clear(IEnumerable<ObstacleController> hazards, float z, float clearance)
        {
            if (hazards == null) return true;
            foreach (var hazard in hazards) if (hazard != null && hazard.gameObject.activeSelf && Mathf.Abs(hazard.transform.position.z - z) < clearance) return false;
            return true;
        }
        private static bool ClearItems(FloorItemController[] items, float z, float clearance)
        {
            if (items == null) return true;
            foreach (var item in items) if (item != null && item.gameObject.activeSelf && Mathf.Abs(item.transform.position.z - z) < clearance) return false;
            return true;
        }
        private static bool ClearGates(MuhanokMapChunk chunk, float z, float clearance)
        {
            if (chunk.Gates != null) foreach (var gate in chunk.Gates) if (gate != null && Mathf.Abs(gate.transform.position.z - z) < clearance + 2f) return false;
            if (chunk.SlideGates != null) foreach (var gate in chunk.SlideGates) if (gate != null && Mathf.Abs(gate.transform.position.z - z) < clearance + 2f) return false;
            return true;
        }

        private static Vector3 LanePoint(int lane, float z, float height)
        {
            RouteSurface.Sample(z, out float x, out float y);
            return new Vector3(lane * 2.2f + x, y + height, z);
        }

        private Stack<ItemPickup> Pool(ItemKind kind) { if (!itemPool.TryGetValue(kind, out var stack)) itemPool[kind] = stack = new Stack<ItemPickup>(); return stack; }
        private ItemPickup Item(ItemKind kind, float z, int lane)
        {
            var stack = Pool(kind);
            var pickup = stack.Count > 0 ? stack.Pop() : BuildItem(kind);
            pickup.transform.position = LanePoint(lane, z, 1.1f); pickup.ResetPickup(); pickup.gameObject.SetActive(true);
            return pickup;
        }
        private ItemPickup BuildItem(ItemKind kind)
        {
            var node = new GameObject("Item_" + kind); node.transform.SetParent(root, false);
            var pickup = node.AddComponent<ItemPickup>(); pickup.Kind = kind;
            Color color = kind == ItemKind.Magnet ? new Color(.85f, .15f, .15f) : kind == ItemKind.DoubleScore ? new Color(1f, .78f, .1f) : new Color(.2f, .6f, 1f);
            Part(PrimitiveType.Sphere, node.transform, Vector3.zero, Vector3.one * .75f, new Color(color.r, color.g, color.b) * .55f + Color.white * .3f);
            if (kind == ItemKind.Magnet)
            {
                Part(PrimitiveType.Cube, node.transform, new Vector3(-.2f, .05f, 0), new Vector3(.14f, .5f, .14f), color);
                Part(PrimitiveType.Cube, node.transform, new Vector3(.2f, .05f, 0), new Vector3(.14f, .5f, .14f), color);
                Part(PrimitiveType.Cube, node.transform, new Vector3(0, -.22f, 0), new Vector3(.54f, .14f, .14f), color);
            }
            else if (kind == ItemKind.DoubleScore)
            {
                Part(PrimitiveType.Cylinder, node.transform, Vector3.zero, new Vector3(.55f, .06f, .55f), color, Quaternion.Euler(90, 0, 0));
            }
            else Part(PrimitiveType.Cube, node.transform, Vector3.zero, new Vector3(.42f, .52f, .12f), color);
            return pickup;
        }
        private FloorItemController BuildBanana()
        {
            var node=new GameObject("RandomBananaPeel");node.transform.SetParent(root,false);
            var item=node.AddComponent<FloorItemController>();item.Kind=FloorItemKind.BananaPeel;
            var yellow=new Color(1,.79f,.08f);
            for(int i=0;i<3;i++)
            {
                var arm=new GameObject("Peel").transform;arm.SetParent(node.transform,false);arm.localRotation=Quaternion.Euler(0,i*120,0);
                Part(PrimitiveType.Capsule,arm,new Vector3(0,.08f,.24f),new Vector3(.17f,.27f,.1f),yellow,Quaternion.Euler(65,0,0));
                Part(PrimitiveType.Capsule,arm,new Vector3(0,.07f,.48f),new Vector3(.13f,.19f,.08f),yellow,Quaternion.Euler(105,0,0));
            }
            Part(PrimitiveType.Cylinder,node.transform,new Vector3(0,.18f,0),new Vector3(.09f,.16f,.09f),new Color(.37f,.21f,.055f),Quaternion.Euler(0,0,12));
            CombineParts(node);return item;
        }
        private ThemedLaneBlock BuildBlock(int theme)
        {
            var node=new GameObject(theme==0?"Mine_RockCart_Block":theme==1?"Prison_LockedGate_Block":"Zoo_FeedCage_Block");node.transform.SetParent(root,false);
            var block=node.AddComponent<ThemedLaneBlock>();block.Theme=theme;
            block.Hazard=node.AddComponent<ObstacleController>();block.Hazard.Kind=ObstacleKind.Barrier;block.Hazard.CollisionHalfWidth=1.101f;
            Color metal=new Color(.21f,.25f,.27f),wood=new Color(.37f,.22f,.11f),yellow=new Color(.87f,.62f,.1f);
            var closure=Part(PrimitiveType.Cube,node.transform,new Vector3(0,1.7f,.38f),new Vector3(2.18f,3.4f,.35f),theme==0?wood:theme==1?metal:new Color(.18f,.32f,.17f));
            closure.name="SolidLaneClosure";
            for(int diagonal=-1;diagonal<=1;diagonal+=2)
                Part(PrimitiveType.Cube,node.transform,new Vector3(0,1.7f,-.3f),new Vector3(.18f,2.8f,.12f),yellow,Quaternion.Euler(0,0,diagonal*39));
            if(theme==0)
            {
                Part(PrimitiveType.Cube,node.transform,new Vector3(0,.85f,0),new Vector3(2,1.1f,1.65f),wood);
                for(int i=0;i<6;i++)Part(PrimitiveType.Sphere,node.transform,new Vector3((i%2-.5f)*.9f,1.6f+i/2*.43f,(i%3-1)*.4f),new Vector3(1.15f,1.15f,1.05f),new Color(.27f,.26f,.23f));
                for(int i=-1;i<=1;i+=2)Part(PrimitiveType.Cylinder,node.transform,new Vector3(i*.8f,.35f,0),new Vector3(.6f,.88f,.6f),metal,Quaternion.Euler(0,0,90));
            }
            else
            {
                Color frame=theme==1?metal:new Color(.19f,.38f,.18f);
                for(int side=-1;side<=1;side+=2)Part(PrimitiveType.Cube,node.transform,new Vector3(side*.98f,1.65f,0),new Vector3(.15f,3.3f,.3f),frame);
                for(int i=0;i<7;i++)Part(PrimitiveType.Cylinder,node.transform,new Vector3(-.84f+i*.28f,1.65f,0),new Vector3(.075f,1.6f,.075f),metal);
                Part(PrimitiveType.Cube,node.transform,new Vector3(0,3.2f,0),new Vector3(2.1f,.2f,.4f),frame);
                Part(PrimitiveType.Cube,node.transform,new Vector3(0,1.7f,-.12f),new Vector3(1.9f,.32f,.12f),yellow);
                if(theme==1)Part(PrimitiveType.Cube,node.transform,new Vector3(0,1.5f,-.25f),new Vector3(.3f,.45f,.22f),wood);
                else
                {
                    Part(PrimitiveType.Cube,node.transform,new Vector3(0,.22f,0),new Vector3(2.1f,.3f,1.6f),frame);
                    for(int i=0;i<3;i++)Part(PrimitiveType.Capsule,node.transform,new Vector3((i-1)*.55f,.85f,.35f),new Vector3(.65f,.65f,.7f),new Color(.67f,.61f,.26f));
                }
            }
            CombineParts(node);return block;
        }
        // Merge same-material primitives once when a pooled object is created, rather than drawing each piece.
        private static void CombineParts(GameObject node)
        {
            var groups=new Dictionary<Material,List<CombineInstance>>();
            foreach(var filter in node.GetComponentsInChildren<MeshFilter>())
            {
                var renderer=filter.GetComponent<MeshRenderer>();if(renderer==null)continue;
                var material=renderer.sharedMaterial;if(!groups.TryGetValue(material,out var list))groups[material]=list=new List<CombineInstance>();
                list.Add(new CombineInstance{mesh=filter.sharedMesh,transform=node.transform.worldToLocalMatrix*filter.transform.localToWorldMatrix});
                renderer.enabled=false;
            }
            foreach(var pair in groups)
            {
                var part=new GameObject("BatchedVisual");part.transform.SetParent(node.transform,false);
                var mesh=new Mesh{name=node.name+"_batched"};mesh.CombineMeshes(pair.Value.ToArray(),true,true);
                part.AddComponent<MeshFilter>().sharedMesh=mesh;part.AddComponent<MeshRenderer>().sharedMaterial=pair.Key;
            }
        }

        private ZooHazard Giraffe(float z)
        {
            var animal = giraffes.Count > 0 ? giraffes.Pop() : BuildGiraffe();
            PlaceAnimal(animal, z, true); return animal;
        }
        private ZooHazard Elephant(float z, int lane)
        {
            var animal = elephants.Count > 0 ? elephants.Pop() : BuildElephant();
            PlaceAnimal(animal, z, true, lane); return animal;
        }
        private static void PlaceAnimal(ZooHazard animal, float z, bool fullRow, int lane = 0)
        {
            RouteSurface.Sample(z, out float x, out float y);
            animal.transform.position = new Vector3(x, y, z);
            for (int i = 0; i < animal.Hazards.Length; i++)
            {
                var hazard = animal.Hazards[i];
                int hazardLane = fullRow ? i - 1 : lane;
                hazard.transform.position = new Vector3(hazardLane * 2.2f + x, y, z);
                hazard.FullExerciseRow = fullRow; hazard.ResetPass(); hazard.gameObject.SetActive(true);
            }
            if (!fullRow && animal.Moving != null)
            {
                // The trunk reaches from the right-hand enclosure into the chosen lane.
                animal.Moving.parent.localPosition = new Vector3(4.6f, 0, 0);
                float length = 4.6f - lane * 2.2f + 1f;
                var trunk = animal.Moving.GetChild(0);
                trunk.localScale = new Vector3(.45f, length * .5f, .45f);
                trunk.localPosition = new Vector3(-length * .5f, 0, 0);
            }
            animal.Preview(-1000f); animal.gameObject.SetActive(true);
        }
        private ZooHazard BuildGiraffe()
        {
            var node = new GameObject("Zoo_Giraffe"); node.transform.SetParent(root, false);
            var animal = node.AddComponent<ZooHazard>(); animal.Giraffe = true;
            var body = new GameObject("Body").transform; body.SetParent(node.transform, false); body.localPosition = new Vector3(5.2f, 0, 0);
            Color yellow = new Color(.95f, .74f, .3f), brown = new Color(.45f, .27f, .12f);
            Part(PrimitiveType.Capsule, body, new Vector3(0, 2.2f, 0), new Vector3(1.3f, 1.1f, 2.4f), yellow, Quaternion.Euler(90, 0, 0));
            for (int i = 0; i < 4; i++) Part(PrimitiveType.Cylinder, body, new Vector3(i < 2 ? -.4f : .4f, 1f, i % 2 == 0 ? -.8f : .8f), new Vector3(.22f, 1f, .22f), yellow);
            for (int i = 0; i < 5; i++) Part(PrimitiveType.Sphere, body, new Vector3(.62f * (i % 2 == 0 ? 1 : -1), 2.3f + (i % 3) * .2f, -.9f + i * .4f), new Vector3(.08f, .35f, .35f), brown);
            // The neck pivots at the shoulder and lowers across all three lanes to slide height.
            var pivot = new GameObject("NeckPivot").transform; pivot.SetParent(body, false); pivot.localPosition = new Vector3(-.4f, .45f, 0);
            Part(PrimitiveType.Cylinder, pivot, new Vector3(0, 3.6f, 0), new Vector3(.38f, 3.6f, .38f), yellow);
            Part(PrimitiveType.Cube, pivot, new Vector3(0, 7.4f, 0), new Vector3(.55f, .6f, 1f), yellow);
            Part(PrimitiveType.Cylinder, pivot, new Vector3(0, 7.85f, .2f), new Vector3(.08f, .25f, .08f), brown);
            Part(PrimitiveType.Cylinder, pivot, new Vector3(0, 7.85f, -.2f), new Vector3(.08f, .25f, .08f), brown);
            for (int i = 0; i < 4; i++) Part(PrimitiveType.Sphere, pivot, new Vector3(.2f, 1.5f + i * 1.4f, 0), new Vector3(.1f, .3f, .3f), brown);
            animal.Moving = pivot; animal.Hazards = Hazards(node.transform, ObstacleKind.Crate, 3);
            return animal;
        }
        private ZooHazard BuildElephant()
        {
            var node = new GameObject("Zoo_Elephant"); node.transform.SetParent(root, false);
            var animal = node.AddComponent<ZooHazard>();
            var body = new GameObject("Body").transform; body.SetParent(node.transform, false); body.localPosition = new Vector3(4.6f, 0, 0);
            Color grey = new Color(.55f, .57f, .6f), tusk = new Color(.95f, .93f, .85f);
            Part(PrimitiveType.Sphere, body, new Vector3(2.4f, 1.9f, 0), new Vector3(3.2f, 2.3f, 2.4f), grey);
            Part(PrimitiveType.Sphere, body, new Vector3(.6f, 2.4f, 0), new Vector3(1.6f, 1.5f, 1.5f), grey);
            Part(PrimitiveType.Cube, body, new Vector3(.9f, 2.5f, .95f), new Vector3(.15f, 1.2f, 1f), grey * .9f);
            Part(PrimitiveType.Cube, body, new Vector3(.9f, 2.5f, -.95f), new Vector3(.15f, 1.2f, 1f), grey * .9f);
            Part(PrimitiveType.Cylinder, body, new Vector3(-.1f, 1.8f, .4f), new Vector3(.12f, .5f, .12f), tusk, Quaternion.Euler(0, 0, 70));
            Part(PrimitiveType.Cylinder, body, new Vector3(-.1f, 1.8f, -.4f), new Vector3(.12f, .5f, .12f), tusk, Quaternion.Euler(0, 0, 70));
            for (int i = 0; i < 4; i++) Part(PrimitiveType.Cylinder, body, new Vector3(i < 2 ? 1.6f : 3.2f, .7f, i % 2 == 0 ? -.7f : .7f), new Vector3(.6f, .7f, .6f), grey);
            // The trunk lies along the ground into the lane: jump over it.
            var pivot = new GameObject("TrunkPivot").transform; pivot.SetParent(body, false); pivot.localPosition = new Vector3(0, .35f, 0);
            var trunk = Part(PrimitiveType.Cylinder, pivot, new Vector3(-3.8f, 0, 0), new Vector3(.45f, 3.8f, .45f), grey, Quaternion.Euler(0, 0, 90));
            trunk.name = "Trunk";
            animal.Moving = pivot; animal.Hazards = Hazards(node.transform, ObstacleKind.Crate, 3);
            return animal;
        }
        private static ObstacleController[] Hazards(Transform parent, ObstacleKind kind, int count)
        {
            var result = new ObstacleController[count];
            for (int i = 0; i < count; i++)
            {
                var hazard = new GameObject("Hazard_" + kind + "_" + i).AddComponent<ObstacleController>();
                hazard.transform.SetParent(parent, false); hazard.Kind = kind; result[i] = hazard;
            }
            return result;
        }
        private static Transform Part(PrimitiveType shape, Transform parent, Vector3 position, Vector3 scale, Color color, Quaternion? rotation = null)
        {
            var part = GameObject.CreatePrimitive(shape);
            var collider = part.GetComponent<Collider>(); if (collider != null) UnityEngine.Object.Destroy(collider);
            part.transform.SetParent(parent, false); part.transform.localPosition = position; part.transform.localScale = scale;
            part.transform.localRotation = rotation ?? Quaternion.identity;
            part.GetComponent<Renderer>().sharedMaterial = Material(color);
            return part.transform;
        }
        private static Material Material(Color color)
        {
            if (materials.TryGetValue(color, out var material) && material != null) return material;
            var shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
            material = new Material(shader) { color = color };
            if (material.HasProperty("_BaseColor")) material.SetColor("_BaseColor", color);
            if (material.HasProperty("_Smoothness")) material.SetFloat("_Smoothness", .25f);
            materials[color] = material; return material;
        }
    }
}
