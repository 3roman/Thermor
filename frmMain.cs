using SqlSugar;
using System;
using System.Drawing;
using System.Windows.Forms;
using Thermor.Model;
using Thermor.Utility;

namespace Thermor
{
    public partial class FrmMain : Form
    {
        public FrmMain()
        {
            InitializeComponent();

            UpdateListView();
        }

        #region 界面
        void UpdateListView()
        {
            var db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = "Data Source=Thermor.db",
                DbType = DbType.Sqlite,
                IsAutoCloseConnection = true
            });
            var velocities = db.Queryable<MediumVelocity>().ToList();
            foreach (var velocity in velocities)
            {
                var item = new ListViewItem(velocity.Medium);
                item.SubItems.Add(velocity.MinVelocity + string.Empty);
                item.SubItems.Add(velocity.MaxVelocity + string.Empty);
                lstMediumVelocity.Items.Add(item);
            }
            var bolts = db.Queryable<BoltHole>().Select(s => s.Bolt).ToArray();
            cbxBolt.Items.AddRange(bolts);
        }

        private static void ResetControls(Control container)
        {
            foreach (Control c in container.Controls)
            {
                if (c is TextBox)
                {
                    ((TextBox)c).Clear();
                    ((TextBox)c).BackColor = SystemColors.Window;
                }
                else if (c is ComboBox)
                {
                    ((ComboBox)c).SelectedIndex = -1;
                }
                ResetControls(c);
            }
        }

        private void RioCondition_CheckedChanged(object sender, EventArgs e)
        {
            if (rioStandardCondition.Checked)
            {
                txtStandardFlowRate.ReadOnly = false;
                txtOperatingFlowRate.ReadOnly = true;
                txtStandardFlowRate.Clear();
                txtOperatingFlowRate.Clear();
                txtStandardFlowRate.Focus();
            }
            else if (rioOperatingCondition.Checked)
            {
                txtStandardFlowRate.ReadOnly = true;
                txtOperatingFlowRate.ReadOnly = false;
                txtStandardFlowRate.Clear();
                txtOperatingFlowRate.Clear();
                txtOperatingFlowRate.Focus();
            }
        }

        private void TabMain_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (tabMain.SelectedTab.Text)
            {
                case @"管径计算":
                    txtFlowRate1.Focus();
                    break;
                case @"管道特性":
                    txtOutDiameter.Focus();
                    break;
                case @"汽水性质":
                    txtPressure.Focus();
                    break;
                case @"杂项功能":
                    txtOperatingPressure.Focus();
                    break;
            }
        }

        private void FrmMain_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                switch (tabMain.SelectedTab.Text)
                {
                    case @"管径计算":
                        CalculateVelocity1();
                        CalculateVelocity2();
                        CalculateEquivalentDiameter();
                        break;
                    case @"管道特性":
                        QueryPipeSpecification();
                        CalculatePipeCharacteristic();
                        break;
                    case @"汽水性质":
                        QuerySteamProperty();
                        break;
                    case @"杂项功能":
                        ConvertFlowRate();
                        OpenHole();
                        CalculateThermalConductivity();
                        break;
                }
            }
            else if (e.KeyCode == Keys.D && ModifierKeys == Keys.Alt)
            {
                ResetControls(tabMain.SelectedTab);
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Escape && ActiveControl is TextBox box)
            {
                box.Clear();
                e.Handled = true;
            }

        }
        #endregion

        #region 管径计算
        private void CalculateVelocity1()
        {
            double.TryParse(txtFlowRate1.Text, out double flowRate);
            double.TryParse(txtDiameter.Text, out double diameter);
            flowRate /= 3600; // m³/h→m³/s
            diameter /= 1000; // mm→m
            var velocity = flowRate / (0.785 * diameter * diameter);
            txtVelocity1.Text = Math.Round(velocity, 1) + string.Empty;
        }
        private void CalculateVelocity2()
        {
            double.TryParse(txtFlowRate2.Text, out double flowRate);
            double.TryParse(txtSectionHeight.Text, out double sectionHeight);
            double.TryParse(txtSectionWidth.Text, out double sectionWidth);
            flowRate /= 3600;
            sectionHeight /= 1000;
            sectionWidth /= 1000;
            var velocity = flowRate / (sectionHeight * sectionWidth);
            txtVelocity2.Text = Math.Round(velocity, 1) + string.Empty;
        }

        private void CalculateEquivalentDiameter()
        {
            double.TryParse(txtDiameter1.Text, out double diameter1);
            double.TryParse(txtDiameter2.Text, out double diameter2);
            double.TryParse(txtDiameter3.Text, out double diameter3);
            var diameter4 = Math.Sqrt(diameter1 * diameter1 + diameter2 * diameter2 + diameter3 * diameter3);
            txtDiameter4.Text = Math.Round(diameter4, 1) + string.Empty;
        }
        #endregion

        #region 管道特性
        private void CalculatePipeCharacteristic()
        {
            double.TryParse(txtOutDiameter.Text, out double outDiameter);
            double.TryParse(txtPipeThickness.Text, out double pipeThickness);
            double.TryParse(txtInsulationThickness.Text, out double insulationThickness);
            double.TryParse(txtPipeLength.Text, out double pipeLength);
            double.TryParse(txtCorrosionAllowance.Text, out double corrosionAllowance);
            double.TryParse(txtMaterialDensity.Text, out double materialDensity);
            double.TryParse(txtDesignTemperature.Text, out double designTemperature);
            outDiameter /= 1000; // mm→m
            pipeThickness /= 1000;
            insulationThickness /= 1000;
            corrosionAllowance /= 1000;

            // 铁皮面积
            var jackerArea = 3.14 * (outDiameter + insulationThickness * 2) * pipeLength;
            // 涂漆面积
            var paintArea = 3.14 * outDiameter * pipeLength;
            // 保温体积
            var insulationVolume = 0.785 * ((outDiameter + insulationThickness * 2) * (outDiameter + insulationThickness * 2)
                - outDiameter * outDiameter) * pipeLength;
            // 管道单重
            var pipeWeight = 0.0246615 * pipeThickness * 1000 * (outDiameter - pipeThickness) * 1000;
            // 奥氏体不锈钢考虑1.015系数
            if (cbxPipeMaterial.Text.Contains("304") || cbxPipeMaterial.Text.Contains("316"))
            {
                pipeWeight *= 1.015;
            }
            // 物料单重
            var materialWeight = materialDensity * 0.785 * (outDiameter - pipeThickness * 2) * (outDiameter - pipeThickness * 2);
            // 充水单重
            var waterWeight = 1000 * 0.785 * (outDiameter - pipeThickness * 2) * (outDiameter - pipeThickness * 2);
            // 保温单重
            var insulationWeight = 200 * 0.785 * ((outDiameter + insulationThickness * 2) * (outDiameter + insulationThickness * 2)
                - outDiameter * outDiameter);
            // 水压试验工况荷
            var hydraulicLoad = (waterWeight + pipeWeight + insulationWeight) * pipeLength;
            // 操作工况荷重
            var operatingLoad = (materialWeight + pipeWeight + insulationWeight) * pipeLength;
            // 管道截面惯性矩，单位mm4
            var innerDiameter = outDiameter - 2 * (pipeThickness + corrosionAllowance);
            var moment = 3.14 / 64 * (Math.Pow(outDiameter * 1000, 4) - Math.Pow(innerDiameter * 1000, 4));
            // 弹性模量，单位为MPa
            double modulus = 0;
            if (cbxPipeMaterial.Text.Contains("20"))
            {
                modulus = LinearInterpolation.Interpolate(ElasticModulus.CS20, designTemperature) * 1000;
            }
            else if (cbxPipeMaterial.Text.Contains("12Cr"))
            {
                modulus = LinearInterpolation.Interpolate(ElasticModulus.AS12Cr1MoVG, designTemperature) * 1000;
            }
            else if (cbxPipeMaterial.Text.Contains("15Cr"))
            {
                modulus = LinearInterpolation.Interpolate(ElasticModulus.AS15CrMoG, designTemperature) * 1000;
            }
            else if (cbxPipeMaterial.Text.Contains("235"))
            {
                modulus = LinearInterpolation.Interpolate(ElasticModulus.CSQ235, designTemperature) * 1000;
            }
            else if (cbxPipeMaterial.Text.Contains("06Cr"))
            {
                modulus = LinearInterpolation.Interpolate(ElasticModulus.SS06Cr19Ni10, designTemperature) * 1000;
            }
            // 管道满水单重 N/m
            var calculatedLoad = hydraulicLoad / pipeLength * 9.8;
            // 计算跨距
            var span = 0.039 * Math.Pow(moment * modulus / calculatedLoad, 0.25);

            txtSupportSpan.Text = Math.Round(span, 1) + string.Empty;
            txtJacketArea.Text = Math.Round(jackerArea, 1) + string.Empty;
            txtPaintArea.Text = Math.Round(paintArea, 1) + string.Empty;
            txtInsulationVolume.Text = Math.Round(insulationVolume, 1) + string.Empty;
            txtTestingLoad.Text = Math.Round(hydraulicLoad, 1) + string.Empty;
            txtOperatingLoad.Text = Math.Round(operatingLoad, 1) + string.Empty;
            if (txtDesignTemperature.Text == string.Empty || modulus <= 0)
            {
                txtSupportSpan.Clear();
            }
            if (materialDensity <= 1)
            {
                txtOperatingLoad.Clear();
            }
        }
        #endregion

        #region 地脚螺栓孔
        private void cbxBolt_SelectedIndexChanged(object sender, EventArgs args)
        {
            if (string.IsNullOrEmpty(cbxBolt.Text))
            {
                txtBoltHole.Clear();
                return;
            }

            var db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = "Data Source=Thermor.db",
                DbType = DbType.Sqlite,
                IsAutoCloseConnection = true
            });
            var boltHole = db.Queryable<BoltHole>().Where(x => x.Bolt == cbxBolt.Text).First();
            txtBoltHole.Text = boltHole.Hole;
        }
        #endregion

        #region 流量转换
        private void ConvertFlowRate()
        {
            double.TryParse(txtOperatingPressure.Text, out double op);
            double.TryParse(txtOperatingTemperature.Text, out double ot);
            double.TryParse(txtOperatingFlowRate.Text, out double of);
            double.TryParse(txtStandardFlowRate.Text, out double sf);
            op += 0.1;  //MPag→MPaA
            ot += 273.15;  //℃→K

            if (rioStandardCondition.Checked)
            {
                var flow = sf * (ot / 273.15) * (0.1 / op);
                txtOperatingFlowRate.Text = Math.Round(flow, 1) + string.Empty;
            }
            else if (rioOperatingCondition.Checked)
            {
                var flow = of * (273.15 / ot) * (op / 0.1);
                txtStandardFlowRate.Text = Math.Round(flow, 1) + string.Empty;
            }
        }
        #endregion

        #region 蒸汽特性
        private void QuerySteamProperty()
        {
            // 清空编辑框
            txtDensity1.Clear();
            txtEnthalpy1.Clear();
            txtViscosity1.Clear();
            txtIsoIndex1.Clear();
            txtVolumeFlow1.Clear();
            txtTotalEnthalpy1.Clear();
            txtDensity2.Clear();
            txtEnthalpy2.Clear();
            txtViscosity2.Clear();
            txtIsoIndex2.Clear();
            txtVolumeFlow2.Clear();
            txtTotalEnthalpy2.Clear();
            txtDensity3.Clear();
            txtEnthalpy3.Clear();
            txtViscosity3.Clear();
            txtIsoIndex3.Clear();
            txtVolumeFlow3.Clear();
            txtTotalEnthalpy3.Clear();

            double retValue = 0.0;
            var range = 0;
            double.TryParse(txtMassFlow.Text, out double massFlow);
            double.TryParse(txtPressure.Text, out double pressure);
            double.TryParse(txtTemperature.Text, out double temperature);
            pressure += 0.1; // MPag→MPaA

            // 使用IFC97标准
            UEwasp.SETSTD_WASP(97);

            // 温度压力都给定
            if (string.Empty != txtPressure.Text && string.Empty != txtTemperature.Text)
            {// 过热汽
                UEwasp.PT2V(pressure, temperature, ref retValue, ref range);
                var volumeFlow = massFlow * 1000 * retValue;
                txtVolumeFlow1.Text = Math.Round(volumeFlow, 1) + string.Empty;
                txtDensity1.Text = Math.Round(1 / retValue, 3) + string.Empty;

                UEwasp.PT2H(pressure, temperature, ref retValue, ref range);
                txtEnthalpy1.Text = Math.Round(retValue, 2) + string.Empty;
                var totalEnthalpy1 = Math.Round(retValue, 2) * massFlow * 1000;
                txtTotalEnthalpy1.Text = totalEnthalpy1 + string.Empty;

                UEwasp.PT2ETA(pressure, temperature, ref retValue, ref range);
                txtViscosity1.Text = Math.Round(retValue * 1000, 3) + string.Empty;

                UEwasp.PT2KS(pressure, temperature, ref retValue, ref range);
                txtIsoIndex1.Text = Math.Round(retValue, 3) + string.Empty;

            }

            // 已知压力
            else if (string.Empty != txtPressure.Text && string.Empty == txtTemperature.Text)
            {
                // 沸点
                UEwasp.P2T(pressure, ref retValue, ref range);
                txtTemperature.Text = string.Format(@"（{0}）", Math.Round(retValue, 1));

                // 饱和汽
                UEwasp.P2VG(pressure, ref retValue, ref range);
                var volumeFlow = massFlow * 1000 * retValue;
                txtVolumeFlow2.Text = Math.Round(volumeFlow, 1) + string.Empty;
                txtDensity2.Text = Math.Round(1 / retValue, 3) + string.Empty;

                UEwasp.P2HG(pressure, ref retValue, ref range);
                txtEnthalpy2.Text = Math.Round(retValue, 2) + string.Empty;
                var totalEnthalpy2 = Math.Round(retValue, 2) * massFlow * 1000;
                txtTotalEnthalpy2.Text = totalEnthalpy2 + string.Empty;

                UEwasp.P2ETAG(pressure, ref retValue, ref range);
                txtViscosity2.Text = Math.Round(retValue * 1000, 3) + string.Empty;

                UEwasp.P2KSG(pressure, ref retValue, ref range);
                txtIsoIndex2.Text = Math.Round(retValue, 3) + string.Empty;

                // 饱和水
                UEwasp.P2VL(pressure, ref retValue, ref range);
                volumeFlow = massFlow * 1000 * retValue;
                txtVolumeFlow3.Text = Math.Round(volumeFlow, 1) + string.Empty;
                txtDensity3.Text = Math.Round(1 / retValue, 3) + string.Empty;

                UEwasp.P2HL(pressure, ref retValue, ref range);
                txtEnthalpy3.Text = Math.Round(retValue, 2) + string.Empty;
                var totalEnthalpy3 = Math.Round(retValue, 2) * massFlow * 1000;
                txtTotalEnthalpy3.Text = totalEnthalpy3 + string.Empty;

                UEwasp.P2ETAL(pressure, ref retValue, ref range);
                txtViscosity3.Text = Math.Round(retValue * 1000, 3) + string.Empty;

                UEwasp.P2KSL(pressure, ref retValue, ref range);
                txtIsoIndex3.Text = Math.Round(retValue, 3) + string.Empty;
            }
            // 已知温度
            else if (string.Empty != txtTemperature.Text && string.Empty == txtPressure.Text)
            {
                UEwasp.T2P(temperature, ref retValue, ref range);
                txtPressure.Text = string.Format(@"（{0}）", Math.Round(retValue, 3) - 0.1);

                // 饱和汽
                UEwasp.T2VG(temperature, ref retValue, ref range);
                var volumeFlow = massFlow * 1000 * retValue;
                txtVolumeFlow2.Text = Math.Round(volumeFlow, 1) + string.Empty;
                txtDensity2.Text = Math.Round(1 / retValue, 3) + string.Empty;

                UEwasp.T2HG(temperature, ref retValue, ref range);
                txtEnthalpy2.Text = Math.Round(retValue, 2) + string.Empty;
                var totalEnthalpy2 = Math.Round(retValue, 2) * massFlow * 1000;
                txtTotalEnthalpy2.Text = totalEnthalpy2 + string.Empty;

                UEwasp.T2ETAG(temperature, ref retValue, ref range);
                txtViscosity2.Text = Math.Round(retValue * 1000, 3) + string.Empty;

                UEwasp.T2KSG(temperature, ref retValue, ref range);
                txtIsoIndex2.Text = Math.Round(retValue, 3) + string.Empty;

                // 饱和水
                UEwasp.T2VL(temperature, ref retValue, ref range);
                volumeFlow = massFlow * 1000 * retValue;
                txtVolumeFlow3.Text = Math.Round(volumeFlow, 1) + string.Empty;
                txtDensity3.Text = Math.Round(1 / retValue, 3) + string.Empty;

                UEwasp.T2HL(temperature, ref retValue, ref range);
                txtEnthalpy3.Text = Math.Round(retValue, 2) + string.Empty;
                var totalEnthalpy3 = Math.Round(retValue, 2) * massFlow * 1000;
                txtTotalEnthalpy3.Text = totalEnthalpy3 + string.Empty;

                UEwasp.T2ETAL(temperature, ref retValue, ref range);
                txtViscosity3.Text = Math.Round(retValue * 1000, 3) + string.Empty;

                UEwasp.T2KSL(temperature, ref retValue, ref range);
                txtIsoIndex3.Text = Math.Round(retValue, 3) + string.Empty;
            }
        }
        #endregion

        #region 开孔大小
        private void OpenHole()
        {
            int.TryParse(txtOutDiameter1.Text, out int outDiameter);
            int.TryParse(txtInsulationThickness1.Text, out int insulationThickness1);
            int.TryParse(txtClearSpacing.Text, out int clearSpacing);
            int.TryParse(txtThermalDisplacement.Text, out int thermalDisplacement);
            var holeSize = outDiameter + 2 * (insulationThickness1 + clearSpacing + thermalDisplacement);
            txtHoleSize.Text = holeSize + string.Empty;
        }
        #endregion

        #region 导热系数
        private void CalculateThermalConductivity()
        {
            double.TryParse(txtMaterialTemperature.Text, out double materialTemperature);
            string InsulationMaterial = string.Empty;
            double ThermalConductivity = 0;
            double tm = (20 + materialTemperature) / 2;
            if (materialTemperature > 0 && materialTemperature <= 250)
            {
                InsulationMaterial = "岩棉制品";
                ThermalConductivity = 0.044 + 0.00018 * (tm - 70);
            }
            else if (materialTemperature > 250 && materialTemperature <= 400)
            {
                InsulationMaterial = "复合硅酸盐制品";
                ThermalConductivity = 0.044 + 0.00015 * (tm - 70);
            }
            else if (materialTemperature > 400)
            {
                InsulationMaterial = "硅酸铝纤维制品";
                ThermalConductivity = 0.056 + 0.0002 * (tm - 70);
            }

            txtInsulationMaterial.Text = InsulationMaterial;
            txtThermalConductivity.Text = ThermalConductivity + string.Empty;
        }
        #endregion
        private void QueryPipeSpecification()
        {
            // TODO 查询管道等级表
        }
    }
}