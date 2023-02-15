﻿using System;

namespace ET
{
    public class L2G_DisconnectGateUnitRequestHandler : AMActorRpcHandler<Scene, L2G_DisconnectGateUnitRequest, G2L_DisconnectGateUnitResponse>
    {
        protected override async ETTask Run(Scene scene, L2G_DisconnectGateUnitRequest request, G2L_DisconnectGateUnitResponse response, Action reply)
        {
            long accountId = request.AccountId;
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.GateLoginLock, accountId))
            {
                //检测该账号是否在Gate中
                var playerCmp = scene.GetComponent<PlayerComponent>();
                Player gateUnit = playerCmp.Get(accountId);
                if (gateUnit is null)   //不在
                {
                    reply();
                    return;
                }

                scene.GetComponent<GateSessionKeyComponent>().Remove(accountId);
                Session gateSession = Game.EventSystem.Get(gateUnit.SessionInstanceId) as Session;
                if (gateSession?.IsDisposed ?? true) 
                {
                    //踢下线
                    gateSession.Send(new A2C_Disconnect() { Error = ErrorCode.ERR_LoginElsewhere });
                    gateSession?.Disconnect().Coroutine();
                }
                gateUnit.SessionInstanceId = 0;
                gateUnit.AddComponent<PlayerOfflineOutTimeComponent>();
            }
            reply();

            await ETTask.CompletedTask;
        }
    }
}
