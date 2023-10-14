<template>

	<el-main style="padding:0 20px;">
		<el-descriptions :column="1" border size="small">			
			<el-descriptions-item label="日志名">{{data.LoggerName}}</el-descriptions-item>
			<el-descriptions-item label="请求名称">{{data.RequestName}}</el-descriptions-item>
			<el-descriptions-item label="请求接口">{{data.RequestPath}}</el-descriptions-item>
			<el-descriptions-item label="请求方法">{{data.RequestMethod}}</el-descriptions-item>
			<el-descriptions-item label="状态代码">{{data.ResponseStatusCode}}</el-descriptions-item>			
			<el-descriptions-item label="请求耗时">{{data.ElapsedMilliseconds}}（毫秒）</el-descriptions-item>
			<el-descriptions-item label="日志时间">{{data.LoggerTime}}</el-descriptions-item>
		</el-descriptions>
		<el-collapse v-model="activeNames" style="margin-top: 20px;">
			<el-collapse-item title="常规" name="1">
				<el-alert :title="data.Message" :type="typeMap[data.level]" :closable="false"></el-alert>
				<el-alert :title="data.exception" :type="typeMap[data.level]" :closable="false"></el-alert>
			</el-collapse-item>
			<el-collapse-item title="详细" name="2">
				<div class="code">
					{
						UserAgent:{{data.UserAgent}},
					    Request: {{data.RequestParams}},					
					    Response: {{data.ResponseData}},
						Eception: {{data.exception}},
				    }
				</div>
			</el-collapse-item>
		</el-collapse>
	</el-main>
</template>

<script>
	export default {
		data() {
			return {
				data: {},
				activeNames: ['1'],
				typeMap: {
					'info': "Information",
					'warn': "Warning",
					'error': "Error"
				}
			}
		},
		methods: {
			setData(data){
				this.data = data
			}
		}
	}
</script>

<style scoped>
	.code {background: #848484;padding:15px;color: #fff;font-size: 12px;border-radius: 4px;}
</style>
