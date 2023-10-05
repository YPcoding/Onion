<template>
	<el-card shadow="never" header="事务待办">
		<el-form ref="form" label-width="180px" label-position="left" style="margin-top:20px;">
			<el-form-item label="有新的待办">
				<el-checkbox v-model="form.new" @change="() => handleCheckboxChange('pushSettings_new', form.new)">短信推送</el-checkbox>
				<el-checkbox v-model="form.new_wx" @change="() => handleCheckboxChange('pushSettings_new_wx', form.new_wx)">微信推送</el-checkbox>
			</el-form-item>
			<el-form-item label="待办有效时剩24小时">
				<el-checkbox v-model="form.timeout" @change="() => handleCheckboxChange('pushSettings_timeout', form.timeout)">短信推送</el-checkbox>
				<el-checkbox v-model="form.timeout_wx" @change="() => handleCheckboxChange('pushSettings_timeout_wx', form.timeout_wx)">微信推送</el-checkbox>
			</el-form-item>
		</el-form>
	</el-card>
</template>

<script>
	export default {
		data() {
			return {
				form: {
					new: false,			
					new_wx:false,
					timeout: false,
					timeout_wx:false
				}
			}
		},
		async mounted() {
			let response = await this.$API.system.user.getUserProfileSettings.get();
			if (response.succeeded) {
				let settings = response.data
				this.form.new = settings.find(e=>e.settingName==="pushSettings_new")?.settingValue=='true'
				this.form.new_wx = settings.find(e=>e.settingName==="pushSettings_new_wx")?.settingValue=='true'
				this.form.timeout = settings.find(e=>e.settingName==="pushSettings_timeout")?.settingValue=='true'
				this.form.timeout_wx = settings.find(e=>e.settingName==="pushSettings_timeout_wx")?.settingValue=='true'
			}
		},
		methods: {
			async handleCheckboxChange(settingName, value) {
				let response = await this.$API.system.user.saveUserProfileSettings.post(settingName,`${value}`,"boolean","false");
				if (response?.succeeded) {
					this.$message.success("设置成功") 
			    }else{
					this.$message.success("设置失败")
				}
            }
		}
	}
</script>

<style>
</style>
