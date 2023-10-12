import * as signalR from "@microsoft/signalr"
import tool from '@/utils/tool'

let accessToken = tool.cookie.get("TOKEN")
const hubConnection = new signalR.HubConnectionBuilder()
    .withUrl("https://localhost:7251/signalRHub", {
        accessTokenFactory: () => {
            return accessToken
        }
    })
    .withAutomaticReconnect() // 启用自动重新连接
    .configureLogging(signalR.LogLevel.Information)
    .build()

export async function startSignalRConnection () {
    try {
        await hubConnection.start()
        console.log("SignalR Hub connected.")
        return true
    } catch (error) {
        console.error(`SignalR Hub connection error: ${error}`)
        return false
    }
}

export default hubConnection