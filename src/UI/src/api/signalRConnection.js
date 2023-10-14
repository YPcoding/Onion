import * as signalR from "@microsoft/signalr"
import tool from '@/utils/tool'

let currentURL = process.env.VUE_APP_API_BASEURL
// 检查是否包含 "/api"
if (currentURL.includes("/api")) {
    // 如果包含 "/api"，则删除它
    currentURL = currentURL.replace("/api", "")
}

// SignalR 配置信息
const signalRConfig = {
    hubUrl: `${currentURL}/signalRHub`,
    accessTokenFactory: () => tool.cookie.get("TOKEN"),
}

// 创建 SignalR 连接
const createSignalRConnection = (config) => {
    const hubConnection = new signalR.HubConnectionBuilder()
        .withUrl(config.hubUrl, {
            accessTokenFactory: config.accessTokenFactory
        })
        .withAutomaticReconnect()
        .configureLogging(signalR.LogLevel.Information)
        .build()

    return hubConnection
}

// 启动 SignalR 连接
const startSignalRConnection = async () => {
    if (hubConnection.state !== signalR.HubConnectionState.Connected) {
        try {
            await hubConnection.start()
            console.log("SignalR Hub connected.")
        } catch (error) {
            console.error(`SignalR Hub connection error: ${error}`)
        }
    }
}

// 订阅消息
const subscribeToReceiveMessage = (methodName, callback) => {
    startSignalRConnection()
    hubConnection.on(methodName, (...args) => {
        if (callback) {
            callback(...args)
        }
    })
}

// 发送消息
const sendMessage = (methodName, ...args) => {
    startSignalRConnection()
    hubConnection.invoke(methodName, ...args)
        .catch((error) => {
            console.error(`Error sending message: ${error}`)
        })
}

// 取消订阅
const unsubscribeFromMessage = (methodName, callback) => {
    hubConnection.off(methodName, callback)
}

// 创建 SignalR 连接
const hubConnection = createSignalRConnection(signalRConfig)

export { hubConnection, startSignalRConnection, subscribeToReceiveMessage, sendMessage, unsubscribeFromMessage }