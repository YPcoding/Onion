<template>
    <el-container>
        <el-aside>
            <el-container>
                <el-header>聊天列表</el-header>
                <el-main style="padding: 10px; display: flex; flex-direction: column;">
                    <div v-for="friend in friends" :key="friend.id"
                        style="margin-bottom: 10px; cursor: pointer; text-align: center; display: flex; align-items: center;">
                        <el-avatar :src="friend.avatar" style="margin-right: 10px;"></el-avatar>
                        <div>
                            {{ friend.name }}
                        </div>
                    </div>
                </el-main>
                <el-footer>Left Footer</el-footer>
            </el-container>
        </el-aside>
        <el-container>
            <el-header>
                <div class="user-profile">
                    <el-avatar :src="currentUser.avatar" style="margin-right: 10px;"></el-avatar>
                    <div>
                        {{ currentUser.name }}
                    </div>
                </div>
            </el-header>
            <el-main class="nopadding" style="overflow-y: auto; padding: 10px;">
                <div v-for="(message, index) in messages" :key="index">
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
                <el-header>Right Header</el-header>
                <el-main>Right Main</el-main>
                <el-footer>Right Footer</el-footer>
            </el-container>
        </el-aside>
    </el-container>
</template>

<script>
export default {
    name: 'layoutLCR',
    data () {
        return {
            friends: [
                { id: 1, name: "Friend 1", avatar: "friend1.jpg" },
                { id: 2, name: "Friend 2", avatar: "friend2.jpg" },
                { id: 3, name: "Friend 3", avatar: "friend3.jpg" },
                // 添加更多好友信息
            ],
            messages: [], // 用于存储聊天消息的数组
            messageInput: '', // 用于输入消息的文本框
            currentUser: {
                avatar: "friend1.jpg",
                name: "哈哈"
            }
        }
    }, methods: {
        // 添加发送的消息
        addOutgoingMessage () {
            if (this.messageInput.trim() !== '') {
                this.messages.push({
                    type: 'incoming', // 消息类型，可以根据需要自定义
                    //type: 'outgoing', // 消息类型，可以根据需要自定义
                    content: this.messageInput,
                    timestamp: "2023-10-15 02:47:56.000"
                })
                this.messages.push({
                    //type: 'incoming', // 消息类型，可以根据需要自定义
                    type: 'outgoing', // 消息类型，可以根据需要自定义
                    content: this.messageInput,
                    timestamp: "2023-10-15 02:47:56.000"
                })
                this.messageInput = '' // 清空输入框
                // 滚动到底部
                this.scrollMessageContainerToBottom()
            }
        },
        scrollMessageContainerToBottom () {
            this.$nextTick(() => {
                const container = document.querySelector('.nopadding')
                container.scrollTop = container.scrollHeight
            })
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
