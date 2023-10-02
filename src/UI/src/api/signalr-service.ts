import * as signalR from "@microsoft/signalr";
import { getToken } from "@/utils/auth";

class signalRService {
  public static instance: signalRService | null = null;
  private hubConnection: signalR.HubConnection | null = null;

  private constructor() {
    // 获取访问令牌
    const token = getToken();

    // 使用访问令牌配置 SignalR 连接
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl("https://localhost:7251/signalRHub", {
        accessTokenFactory: () => {
          // 返回访问令牌
          return token.accessToken;
        }
      })
      .build();

    // 启动连接
    this.hubConnection.start().catch(err => {
      console.error("Error starting SignalR connection: " + err);
    });
  }

  public static getInstance(): signalRService {
    if (!this.instance) {
      this.instance = new signalRService();
    }
    return this.instance;
  }

  public getHubConnection(): signalR.HubConnection {
    if (!this.hubConnection) {
      throw new Error("SignalR Hub connection is not established.");
    }
    return this.hubConnection;
  }
}

export default signalRService;
