using System;
using System.Text;
using System.Collections.Generic;
using Sandbox.Common.ObjectBuilders;
using SpaceEngineers.ObjectBuilders.ObjectBuilders;
using Sandbox;
using Sandbox.Definitions;
using Sandbox.Game;
using Sandbox.Game.Entities;
using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI;
using Sandbox.ModAPI.Interfaces;
using Sandbox.ModAPI.Interfaces.Terminal;
using SpaceEngineers.Game.ModAPI;
using VRage;
using VRage.Game;
using VRage.Game.Entity;
using VRage.Game.Components;
using VRage.Game.ModAPI;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Input;
using VRage.ModAPI;
using VRage.ObjectBuilders;
using VRage.Utils;
using VRageMath;
using BlendTypeEnum = VRageRender.MyBillboard.BlendTypeEnum;

namespace Dhr.HEAmmo
{
    [MySessionComponentDescriptor(MyUpdateOrder.NoUpdate)]
    
    class PhosExplosionHandler : MySessionComponentBase
    {
        const int projectileCount = 54;

        // Definitions
        MyDefinitionId wepDef = new MyDefinitionId(typeof(MyObjectBuilder_WeaponDefinition), "FlarePistolGun");
        MyDefinitionId magDef = new MyDefinitionId(typeof(MyObjectBuilder_AmmoMagazine), "PhosphorusClip");
        MyDefinitionId projDef = new MyDefinitionId(typeof(MyObjectBuilder_AmmoDefinition), "Phosphorus");
        public override void LoadData()
        {
            MyExplosions.OnExplosion += OnExplosion;
        }

        protected override void UnloadData()
        {
            MyExplosions.OnExplosion -= OnExplosion;
        }
        void OnExplosion(ref MyExplosionInfo info)
        {
            if (info.HitEntity is IMyWarhead)
            {
                IMyWarhead warhead = info.HitEntity as IMyWarhead;

                if (warhead.BlockDefinition.SubtypeId == "SmallPhosphorusWarhead")
                {
                    Vector3D location = warhead.WorldMatrix.Translation;

                    Random rand = new Random();

                    // Create projectiles
                    for (int i = 0; i < projectileCount; i++)
                    {
                        Vector3D forward = (new Vector3D(rand.Next(-50, 50), rand.Next(-50, 50), rand.Next(-50, 50))).Normalized();
                        Vector3D up = forward.Cross(warhead.WorldMatrix.Up);

                        MyObjectBuilder_Missile missileBuilder = new MyObjectBuilder_Missile()
                        {
                            WeaponDefinitionId = wepDef,
                            AmmoMagazineId = magDef,
                            EntityDefinitionId = projDef,
                            Owner = warhead.OwnerId,
                            OriginEntity = warhead.EntityId,
                            LinearVelocity = forward * 20,
                            PositionAndOrientation = new MyPositionAndOrientation(location, forward, up),

                            PersistentFlags = (MyPersistentEntityFlags2.Enabled | MyPersistentEntityFlags2.InScene),
                        };

                        MyAPIGateway.Entities.CreateFromObjectBuilderAndAdd(missileBuilder);                 
                    }
                }
            }
        }
    }
}
