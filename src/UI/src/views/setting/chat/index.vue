<template>
    <el-container>
        <el-aside>
            <el-container>
                <el-header>聊天列表</el-header>
                <el-main style="padding: 10px; display: flex; flex-direction: column;">
                    <div v-for="friend in friends" :key="friend.userId" @click="selectChatUser(friend)"
                        style="margin-bottom: 10px; cursor: pointer; text-align: center; display: flex; align-items: center;">
                        <el-avatar :src="friend.profilePictureDataUrl" style="margin-right: 10px;"></el-avatar>
                        <div>
                            {{ friend.userName }}
                        </div>
                    </div>
                </el-main>
                <el-footer>左边底部</el-footer>
            </el-container>
        </el-aside>
        <el-container>
            <el-header>
                <div class="user-profile">
                    <el-avatar :src="currentChatUser.profilePictureDataUrl" style="margin-right: 10px;"></el-avatar>
                    <div>
                        {{ currentChatUser.userName }}
                    </div>
                </div>
            </el-header>
            <el-main class="nopadding" style="overflow-y: auto; padding: 10px;">
                <div v-for="(message, index) in getChatHistory(currentChatUser.userId)" :key="index">
                    <div class="message incoming" v-if="message.type === 'incoming'">
                        <div class="message-content">
                            <p>{{ message.content }}</p>
                            <p class="message-timestamp-left">{{ message.timestamp }}</p>
                        </div>
                    </div>
                    <div class="message outgoing" v-else-if="message.type === 'outgoing'">
                        <div class="message-content">
                            <p>{{ message.content }}</p>
                            <p class="message-timestamp-right">{{ message.timestamp }}</p>
                        </div>
                    </div>
                </div>
            </el-main>
            <el-footer>
                <el-input v-model="messageInput" placeholder="输入消息..." clearable @keyup.enter="addOutgoingMessage"
                    class="custom-input">
                    <template v-slot:append>
                        <!-- 使用 el-button 组件来实现按钮 -->
                        <el-button class="custom-button" icon="el-icon-position" @click="addOutgoingMessage"></el-button>
                    </template>
                </el-input>
            </el-footer>
        </el-container>
        <el-aside>
            <el-container>
                <el-header>功能列表</el-header>
                <el-main>功能区</el-main>
                <el-footer>右边底部</el-footer>
            </el-container>
        </el-aside>
    </el-container>
</template>

<script>
export default {
    name: 'layoutLCR',
    data () {
        return {
            chatHistories: {}, // 保存多个聊天记录
            friends: [
                {
                    id: null,
                    userId: 0,
                    userName: "",
                    profilePictureDataUrl: "",
                    isLive: true
                }
                // 添加更多好友信息
            ],
            messageInput: '', // 用于输入消息的文本框
            currentChatUser: {
                profilePictureDataUrl: "",
                userName: "",
                userId: ""
            },
            message: {
                type: "",           // 消息类型，例如 'incoming' 或 'outgoing'
                content: "",        // 消息内容
                contentType: "",    // 内容类型，例如 'text' 或 'image'
                timestamp: new Date(),  // 消息时间戳
                sender: {
                    userId: "",        // 发送者用户ID
                    userName: "",      // 发送者用户名
                    profilePictureDataUrl: ""  // 发送者头像
                }
            },
        }
    }, async mounted () {
        this.queryChatList() //查询聊天列表
        this.$SubscribeToReceiveMessage("ReceivePrivateMessage", this.handleMessage)//订阅私聊信息
    }, methods: {
        // 添加消息到指定聊天
        addMessageToChat (chatId, message) {
            if (!this.chatHistories[chatId]) {
                this.chatHistories[chatId] = [] // 如果聊天不存在，初始化一个空数组
            }
            this.chatHistories[chatId].push(message)
            this.$TOOL.data.set("CHAT_HISTORIES", this.chatHistories)
        },
        // 获取特定聊天的消息记录
        getChatHistory (chatId) {
            this.chatHistories = this.$TOOL.data.get("CHAT_HISTORIES")
            if (this.chatHistories[chatId]) {
                return this.chatHistories[chatId]
            }
            return []
        },
        // 创建一个回调函数来处理接收到的消息
        handleMessage (message) {
            let msg = JSON.parse(message)
            let chatId = msg.sender.userId
            if (this.currentChatUser.userId === "") {
                this.currentChatUser.profilePictureDataUrl = msg.sender.profilePictureDataUrl
                this.currentChatUser.userName = msg.sender.userName
                this.currentChatUser.userId = msg.sender.userId
            }

            this.addMessageToChat(chatId, msg)
            this.scrollMessageContainerToBottom()
        },
        //查询聊天列表
        async queryChatList () {
            let response = await await this.$API.system.chats.list.post({
                pageNumber: 1,
                pageSize: 10,
                orderBy: "Id",
                sortDirection: "Descending"
            })
            if (response.data.items) {
                this.friends = response.data.items
                if (response.data.items.length > 1) {
                    this.currentChatUser.profilePictureDataUrl = this.friends[1].profilePictureDataUrl
                    this.currentChatUser.userName = this.friends[1].userName
                    this.currentChatUser.userId = this.friends[1].userId
                }
            }
        },
        // 添加发送的消息
        addOutgoingMessage () {
            if (this.messageInput.trim() === '') {
                return // 什么都不做，因为输入是空的
            }

            const message = this.createOutgoingMessage(this.messageInput)
            this.addMessageToChat(this.currentChatUser.userId, message)
            this.sendMessageToServer(message)

            this.messageInput = '' // 清空输入框
            this.scrollMessageContainerToBottom()
        },
        //创建发送消息
        createOutgoingMessage (content) {
            const date = new Date()
            return {
                type: 'outgoing',
                content: content,
                contentType: 'text',
                timestamp: this.formatDate(date),
            }
        },
        //发送消息到服务器
        sendMessageToServer (message) {
            const messageJson = JSON.stringify(message)
            this.$SendMessage('SendPrivateMessageAsync', this.currentChatUser.userId, messageJson)
        },
        //格式化时间
        formatDate (date) {
            return date.toLocaleString()
        },
        //聊天内容滚动到底部
        scrollMessageContainerToBottom () {
            this.$nextTick(() => {
                const container = document.querySelector('.nopadding')
                container.scrollTop = container.scrollHeight
            })
        },
        //选择聊天用户
        selectChatUser (row) {
            this.currentChatUser.profilePictureDataUrl = row.profilePictureDataUrl
            this.currentChatUser.userName = row.userName
            this.currentChatUser.userId = row.userId
        },
    },
}
</script>

<style>
/* 收到的消息样式 */
.message.incoming {
    text-align: left;
    margin-bottom: 10px;
}

/* 发送的消息样式 */
.message.outgoing {
    text-align: right;
    margin-bottom: 10px;
}


/* 共享的样式 */
.message-content {
    padding: 10px;
    border-radius: 10px;
    display: inline-block;
    max-width: 70%;
    word-wrap: break-word;
    text-align: left;
    font-size: 16px;
    /* 调整字体大小 */
    color: #333;
    /* 调整字体颜色 */
}

/* 左边聊天内容的背景色 */
.message.incoming .message-content {
    background-color: #E6F7FF;
    font-size: 16px;
    /* 调整字体大小 */
    color: #000;
    /* 调整字体颜色 */
}

/* 右边聊天内容的背景色 */
.message.outgoing .message-content {
    background-color: #e0e0e0;
    font-size: 16px;
    /* 调整字体大小 */
    color: #000;
    /* 调整字体颜色 */
}

/* 消息时间戳样式 */
.message-timestamp-left {
    font-size: 12px;
    /* 调整时间戳的字体大小 */
    color: #888;
    /* 根据需要设置时间戳的颜色 */
    text-align: left;
    /* 左对齐 */
}

/* 消息时间戳样式 */
.message-timestamp-right {
    font-size: 12px;
    /* 调整时间戳的字体大小 */
    color: #888;
    /* 根据需要设置时间戳的颜色 */
    text-align: right;
    /* 左对齐 */
}

/* 用户头像和昵称样式 */
.user-profile {
    display: flex;
    align-items: center;
}

.custom-input input {
    font-size: 16px;
    /* 设置文字大小 */
    color: #333;
    /* 设置文字颜色 */
    /* 其他样式，如字体、边框、背景色等，可以根据需要自定义 */
}
</style>
