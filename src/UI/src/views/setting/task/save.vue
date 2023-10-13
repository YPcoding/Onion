<template>
	<el-dialog :title="titleMap[mode]" v-model="visible" :width="600" destroy-on-close @closed="$emit('closed')">
		<el-form :model="form" :rules="rules" ref="dialogForm" label-width="100px" label-position="left">
			<el-form-item label="任务名称" prop="jobName">
				<el-input v-model="form.jobName" placeholder="计划任务名称" clearable></el-input>
			</el-form-item>
			<el-form-item label="执行类" prop="jobGroup">
				<el-select v-model="form.jobGroup" placeholder="请选择执行类">
					<el-option
                        v-for="item in JobGroupOptions"
                        :key="item.value"
                        :label="item.label"
                        :value="item.value">
	                </el-option>
                </el-select>
			</el-form-item>
			<el-form-item label="任务参数">
				<el-input v-model="form.data" prop="data" placeholder="定时任务参数" clearable></el-input>
			</el-form-item>
			<el-form-item label="参数示例" v-if="form.parameterJson!=null">
				<sc-code-editor ref="editor" v-model="form.parameterJson" mode="javascript" :height="200"></sc-code-editor>
				<div style="margin-top: 15px;">
					<el-button type="primary" @click="getValue">复制Json到任务参数</el-button>
				</div>
			</el-form-item>
			<el-form-item label="定时规则" prop="cronExpression">
				<sc-cron v-model="form.cronExpression" placeholder="请输入Cron定时规则" clearable :shortcuts="shortcuts"></sc-cron>
			</el-form-item>
			<el-form-item label="是否启用" prop="status">
				<el-switch v-model="form.status" active-value="Normal" inactive-value="Paused"></el-switch>
			</el-form-item>
		</el-form>
		<template #footer>
			<el-button @click="visible=false" >取 消</el-button>
			<el-button type="primary" :loading="isSaveing" @click="submit()">保 存</el-button>
		</template>
	</el-dialog>
</template>

<script>
	import scCron from '@/components/scCron';
	import { defineAsyncComponent } from 'vue';
	const scCodeEditor = defineAsyncComponent(() => import('@/components/scCodeEditor'));

	export default {
		components: {
			scCron,
			scCodeEditor
		},
		emits: ['success', 'closed'],
		data() {
			return {
				mode: "add",
				titleMap: {
					add: '新增计划任务',
					edit: '编辑计划任务'
				},
				form: {
					id:"",
					jobName: "",
					jobGroup: "",
					data:null,
					cronExpression: "",
					status: "Inactive",
					parameterJson:null
				},
				rules: {
					jobName:[
						{required: true, message: '请填写标题'}
					],
					jobGroup:[
						{required: true, message: '请填写执行类'}
					],
					cronExpression:[
						{required: true, message: '请填写定时规则'}
					],
					data:[
					    { required: true, message: '请填写正确的Json格式' },
						{
                            validator: (rule, value, callback) => {
                                if (value) {
									try {
										console.log(value)
                                        JSON.parse(value);
										console.log(111111)
										callback();
                                    } catch (e) {
										console.log(e)
										console.log(222222)
                                    }
                                } else {
                                    callback();
									console.log(33333)
                                }
                            },
                        },
					]
				},
				visible: false,
				isSaveing: false,
				shortcuts: [
					{
						text: "每天8点和12点 (自定义追加)",
						value: "0 0 8,12 * * ?"
					}
				],
				JobGroupOptions:[]
			}
		},
		async mounted() {
			var response = await this.$API.system.tasks.jobGroups.get();
			if (response?.succeeded) {
				this.JobGroupOptions = response.data
			} 
		},	
		methods: {
			//显示
			open(mode='add'){
				this.mode = mode;
				this.visible = true;
				return this;
			},
			//表单提交方法
			async submit(){
				this.$refs.dialogForm.validate(async (valid) => {
					if (valid) {
						this.isSaveing = true;
						var response = null;
						if (this.mode==='add') {
							response = await this.$API.system.tasks.add.post(this.form);
						}else{
							response = await this.$API.system.tasks.update.put(this.form);
						}
			            if (response?.succeeded) {
					        this.isSaveing = false;
						    this.visible = false;
							this.$message.success("操作成功")
							this.$emit('success', this.form, this.mode)
			            } else {
							this.$message.error(response.message)
						}
					}
				})
			},
			getValue(){
				this.$message("复制成功")
				var v = this.$refs.editor.coder.getValue()
				const json = v.replace(/\/\*[\s\S]*?\*\/|\/\/.*/g, '');
				this.form.data =json
			},
			//表单注入数据
			setData(data){
				this.form.scheduledJobId = data.scheduledJobId
				this.form.jobName = data.jobName
				this.form.jobGroup = data.jobGroup		
				this.form.data = data?.data			
				this.form.parameterJson = data?.data	
				this.form.cronExpression = data.cronExpression	
				this.form.status = data.status
				this.form.concurrencyStamp = data.concurrencyStamp 
			}
		},
		watch: {
			'form.jobGroup'(val){
				this.form.parameterJson = null
				const job = this.JobGroupOptions.find(item => item.value ===val );
				if (job && job.parameterJson) {			
					this.form.parameterJson = job.parameterJson
					this.form.parameterJson = "/**参数使用说明\r\n"+job.description+"*/\r\n" + this.form.parameterJson
				}
			}
		}
	}
</script>

<style>
</style>
