/*使用说明 注意：此脚本只适用于单转子并安装在侧面的结构*/
/*1、安装一个控制座椅，自定义名称使其包含“-cb” 示例：“控制座椅1-cb”*/
/*2、在左侧或右侧安装一个转子，使其0度位置正对飞船的上方向并将其编组，推进器安装时喷口也需对准0度方向，编组名称包含“-veg”和一个安装位置标记，左标记为“-L”，右标记为-R 示例：“引擎组1-veg-L”*/
/*3、安装编程块并复制此脚本点击重置代码*/
/*4、尽可能调大转子的刹车和扭力*/
/*运行模式*/
/*在编程块中的参数框输入参数点击运行或直接在快捷栏设置运行参数来执行脚本的各种功能*/
/*运行参数"1":无重力模式*/
/*运行参数"2":有重力模式*/
/*运行参数"off":关闭引擎并复位*/
/*运行参数"r":重新分配推力(高度发生较大变化时使用)*/
#region 常量定义区
// 控制方块名称标签
public const string CONTROLER_BLOCK = "-cb";
//矢量喷口控制转子的转速
public const float ROTATE_MAX_SPEED = 2f;
// 矢量引擎组名称标签
public const string VECTOR_ENGINE_GROUP_NAME = "-veg";
// 安装位置标签左
public const string INSTALLATION_SITE_LEFT = "-L";
// 安装位置标签右
public const string INSTALLATION_SITE_RIGTH = "-R";
//喷口启动误差范围
public const float PROPELLER_ON_ERRORRANGE = 0.01f;
//惯性矢量精度取值阈值
public const float INERTIA_VECTOR_PRECISION = 0.01f;
//转子精度误差范围
public const float MOTOR_ACCURACY = 0.05f;
//转子默认关闭朝向
public readonly int[] MOTOR_DEFAULT = new int[] { -1, 0 };
#endregion

#region 变量定义区
//控制块
public IMyShipController ctrlBlock;
//矢量引擎组
public List<VectorEngineGroup> vectorEngineGroups = new List<VectorEngineGroup>();
//调试面板
public IMyTextPanel Lcd;
//执行参数
public string exec_para = string.Empty;
//上次执行参数
public string oldExec = string.Empty;
//重力调整
public float antigravity;
//用于惯性抑制与加速的最大推力
public float surplus_Thrust;
#endregion

#region 类型定义区
// 矢量引擎组
public class VectorEngineGroup
{
    //标记转子是否到位
    public bool isSuccess;
    // 安装位置
    public LocationEnum location;
    // 转子
    public IMyMotorStator motor;
    // 推进器
    public List<IMyThrust> thrusters = new List<IMyThrust>();
    // 转子转速
    public float rotateSpeed;
    // 转子最高转速
    public float rotateMaxSpeed;
    // 转子当前角度
    public float Angle { get { return motor.Angle; } }
    //引擎组总推力
    public float ThrustSum { get { return thrusters.Sum(x => x.MaxThrust); } }
    //标准推力矢量长度为10，用于计算推力夹角，不能视作实际推力
    public Vector_2D ThrustVector { get { return VEHelp.AngleToVector(Angle, 10); } }
    //构造矢量引擎实体
    public VectorEngineGroup(IMyMotorStator _motor, List<IMyThrust> _thrusters, LocationEnum _location)
    {
        rotateSpeed = 0;
        rotateMaxSpeed = ROTATE_MAX_SPEED;
        location = _location;
        motor = _motor;
        thrusters = _thrusters;
    }

    // 设置转子朝向
    public void SetMotorOrientation(Vector_2D _vector)
    {
        if (_vector == null)
        {
            motor.TargetVelocityRad = 0;
            return;
        }
        //设置未到位状态
        isSuccess = false;
        //转子目标角度区分
        Vector_2D vector;
        switch (location)
        {
            case LocationEnum.Left:
                vector = new Vector_2D(_vector.Y, _vector.Z);
                break;
            case LocationEnum.Rigth:
                vector = new Vector_2D(_vector.Y, -_vector.Z);
                break;
            default:
                throw new Exception("引擎组没有位置标记");
        }
        //检测矢量是否成立
        if (vector.Quadrant != 0)
        {
            int rl2;
            float target = (float)ThrustVector.GetVectorAngle(out rl2, vector, false);
            if (target < 0.2f)
                rotateSpeed = rotateMaxSpeed * (target / 0.2f);
            else
                rotateSpeed = rotateMaxSpeed;
            if (target > MOTOR_ACCURACY)
            {
                if (target < PROPELLER_ON_ERRORRANGE)
                    isSuccess = true;
                else
                    isSuccess = false;
                //相对角度小于指定值时允许启动喷口
                if (rl2 > 0)
                    motor.TargetVelocityRad = -rotateSpeed;
                else
                    motor.TargetVelocityRad = rotateSpeed;
            }
            else
            {
                motor.TargetVelocityRad = 0f;
                isSuccess = true;
            }
        }
        else
        {
            motor.TargetVelocityRad = 0f;
        }
    }
    //设置推进器推力
    public void SetThrusterThrust(float power)
    {
        foreach (var i in thrusters)
        {
            i.ThrustOverridePercentage = power;
        }
    }
    //设置推进器推力
    public void SetThrusterThrust(float power, int groups)
    {
        float power1 = power / groups;
        float power2 = power1 / thrusters.Count;
        foreach (var i in thrusters)
        {
            i.ThrustOverride = power2;
        }
    }
    public void ThrustON_Off(bool para)
    {
        foreach (var i in thrusters)
        {
            i.Enabled = para;
        }
    }
    // 位置枚举
    public enum LocationEnum : int { Left = 1, Rigth = 2 }
}
// 2D矢量(自定义)
public class Vector_2D
{
    public Vector_2D(double y, double z)
    {
        this.y = y;
        this.z = z;
    }
    public Vector_2D(Vector3D vector3D)
    {
        y = vector3D.Y;
        z = vector3D.Z;
    }
    public Vector_2D(Vector3D vector3D, float i)
    {
        y = VEHelp.Threshold_Screening(vector3D.Y, i);
        z = VEHelp.Threshold_Screening(vector3D.Z, i);
    }
    private double y;
    // 坐标投影:y轴
    public double Y { get { return y; } set { vector = null; y = value; } }
    private double z;
    // 坐标投影:z轴
    public double Z { get { return z; } set { vector = null; z = value; } }
    public double? vector;
    // 矢量长度
    public double Vector
    {
        get
        {
            if (vector == null)
                vector = Math.Sqrt(Math.Pow(y, 2) + Math.Pow(z, 2));
            return (double)vector;

        }
    }
    // 矢量的象限
    public int Quadrant
    {
        get
        {
            if (Y > 0 && Z > 0) return 1;
            if (Y < 0 && Z > 0) return 2;
            if (Y < 0 && Z < 0) return 3;
            if (Y > 0 && Z < 0) return 4;
            if (Y > 0 && Z == 0) return -1;
            if (Y == 0 && Z > 0) return -2;
            if (Y < 0 && Z == 0) return -3;
            if (Y == 0 && Z < 0) return -4;
            return 0;
        }
    }

    // 获取矢量夹角(默认使用与y轴的夹角),out参数输出两条矢量的相对位置-1左边，1右边
    public double GetVectorAngle(out int rl, Vector_2D _v = null, bool d = true)
    {
        Vector_2D v = _v == null ? new Vector_2D(1, 0) : _v;
        if (Y == 0 && Z == 0 || v.Y == 0 && v.Z == 0) { rl = 0; return 0; }
        double _angle = Math.Acos((Y * v.Y + Z * v.Z) / (Vector * v.Vector));
        double cross = v.Z * Y - v.Y * Z;
        double result;
        if (cross > 0)
        {
            //逆时针方向
            rl = -1;
            result = Math.PI * 2 - _angle;
        }
        else if (cross < 0)
        {
            //顺时针方向
            rl = 1;
            result = _angle;
        }
        else
        {
            //共轴
            rl = 0;
            double lv = (this.Vector > v.Vector) ? this.Vector : v.Vector;
            Vector_2D ve = this + v;
            if (ve.Vector > lv)
            {
                result = 0;
                _angle = 0;
            }
            else
                result = Math.PI;
        }
        if (d)
            return result;
        else
            return _angle;
    }

    //矢量投影
    public static Vector_2D VectorConversion(Vector_2D direction, double vector)
    {
        if (direction.Y == 0 && direction.Z == 0) { return new Vector_2D(0, 0); }
        if (direction.Y < 0 && direction.Z == 0) { return new Vector_2D(-vector, 0); }
        if (direction.Y > 0 && direction.Z == 0) { return new Vector_2D(vector, 0); }
        if (direction.Y == 0 && direction.Z < 0) { return new Vector_2D(0, -vector); }
        if (direction.Y == 0 && direction.Z > 0) { return new Vector_2D(0, vector); }
        double a = direction.Y;
        double b = direction.Z;
        double c = direction.Vector;
        double bc = Math.Asin(a / c);
        double c2 = vector;
        double a2 = c2 * Math.Sin(bc);
        double b2 = c2 * Math.Cos(bc);
        if (b < 0) b2 = -b2;
        return new Vector_2D(a2, b2);
    }
    // 加号运算符重载
    public static Vector_2D operator +(Vector_2D a, Vector_2D b) => new Vector_2D(a.Y + b.Y, a.Z + b.Z);
    public static Vector_2D operator -(Vector_2D a, Vector_2D b) => new Vector_2D(a.Y - b.Y, a.Z - b.Z);

}
// 工具类
public static class VEHelp
{
    //将小于参数2的数视为0
    public static double Threshold_Screening(double para, float thresholdValue)
    {
        return (double)Math.Abs(para) > thresholdValue ? para : 0;
    }
    //用于将重力矢量旋转90度
    public static Vector_2D Vector90Rotate(Vector_2D vector, bool direction)
    {
        double y = Math.Abs(vector.Z);
        double z = Math.Abs(vector.Y);
        switch (vector.Quadrant)
        {
            case 1:
            case -1:
                y = -y;
                break;
            case 2:
            case -2:
                y = -y;
                z = -z;
                break;
            case 3:
            case -3:
                z = -z;
                break;
        }
        if (direction)
            return new Vector_2D(y, z);
        else
            return new Vector_2D(-y, -z);

    }
    //角度与标量合成矢量
    public static Vector_2D AngleToVector(double angle, double length)
    {
        if (angle == 0) return new Vector_2D(length, 0);
        if (angle == Math.PI / 2) return new Vector_2D(0, length);
        if (angle == Math.PI) return new Vector_2D(-length, 0);
        if (angle == Math.PI * 1.5) return new Vector_2D(0, -length);
        if (angle > Math.PI * 1.5)
        {
            double _angle = angle - Math.PI * 1.5;
            double y = length * Math.Sin(_angle);
            double z = length * Math.Cos(_angle);
            return new Vector_2D(y, -z);
        }
        else if (angle > Math.PI)
        {
            double _angle = angle - Math.PI;
            double z = length * Math.Sin(_angle);
            double y = length * Math.Cos(_angle);
            return new Vector_2D(-y, -z);
        }
        else if (angle > Math.PI / 2)
        {
            double _angle = angle - Math.PI / 2;
            double y = length * Math.Sin(_angle);
            double z = length * Math.Cos(_angle);
            return new Vector_2D(-y, z);
        }
        else
        {
            double _angle = angle;
            double z = length * Math.Sin(_angle);
            double y = length * Math.Cos(_angle);
            return new Vector_2D(y, z);
        }
    }
}
#endregion

#region 动态工具方法
// 获取方块编组
public List<IMyBlockGroup> GetGroups(string groupName)
{
    List<IMyBlockGroup> groups = new List<IMyBlockGroup>();
    GridTerminalSystem.GetBlockGroups(groups);
    if (groups.Count <= 0) { return groups; }
    groups = groups.Where(fu => fu.Name.Contains(groupName)).ToList();
    return groups;
}
//克服重力需要的推力
public float GetOvercomeGravity(Vector_2D vector)
{
    var totalMass = ctrlBlock.CalculateShipMass().PhysicalMass;
    IMyThrust thrust = vectorEngineGroups.FirstOrDefault().thrusters.FirstOrDefault();
    return totalMass * (float)vector.Vector / (thrust.MaxEffectiveThrust / thrust.MaxThrust);
}
#endregion

#region 执行区
//无重力模式
public void AgravicModel()
{
    Vector3D inertia = Vector3D.TransformNormal(ctrlBlock.GetShipVelocities().LinearVelocity, MatrixD.Transpose((ctrlBlock as IMyEntity).WorldMatrix));
    Vector3D input = ctrlBlock.MoveIndicator;
    Vector_2D iv = new Vector_2D(VEHelp.Threshold_Screening(inertia.Y, INERTIA_VECTOR_PRECISION), VEHelp.Threshold_Screening(inertia.Z, INERTIA_VECTOR_PRECISION));
    Vector_2D cv = Vector_2D.VectorConversion(new Vector_2D(-input.Y, -input.Z), 115);
    Vector_2D tarVector = Vector_2D.VectorConversion(iv + cv, 100);
    foreach (var item in vectorEngineGroups)
    {
        if (tarVector.Vector > 0.5)
        {
            item.SetMotorOrientation(tarVector);
            double power;
            if (tarVector.Vector > 10)
                power = 1;
            else
                power = tarVector.Vector / 10;
            if (item.isSuccess)
                item.SetThrusterThrust((float)power);
            else
                item.SetThrusterThrust(0f);
        }
        else
        {
            item.SetThrusterThrust(0f);
            item.SetMotorOrientation(null);
        }
    }
}

//重力模式
public void gravityModel()
{
    double maxThrust = surplus_Thrust;
    Vector3D inertia = Vector3D.TransformNormal(ctrlBlock.GetShipVelocities().LinearVelocity, MatrixD.Transpose((ctrlBlock as IMyEntity).WorldMatrix));
    Vector3D input = ctrlBlock.MoveIndicator;
    Vector3D gravity = Vector3D.TransformNormal(ctrlBlock.GetNaturalGravity(), MatrixD.Transpose((ctrlBlock as IMyEntity).WorldMatrix));
    Vector_2D _gravity = new Vector_2D(gravity, 0.005f);
    Vector_2D _inertia = new Vector_2D(inertia, INERTIA_VECTOR_PRECISION);
    Vector_2D _input = new Vector_2D(-input.Y, -input.Z);
    float g = GetOvercomeGravity(_gravity);
    if (_inertia.Vector < 20)
        maxThrust = surplus_Thrust * (_inertia.Vector / 20);
    Vector_2D n_gravity = Vector_2D.VectorConversion(_gravity, g);
    Vector_2D n_inertia = Vector_2D.VectorConversion(_inertia, maxThrust);
    Vector_2D n_input = Vector_2D.VectorConversion(_input, surplus_Thrust);
    double tarY = n_input.Y == 0 ? n_inertia.Y : n_input.Y;
    double tarZ = n_input.Z == 0 ? n_inertia.Z : n_input.Z;
    Vector_2D tarVector = new Vector_2D(tarY, tarZ);
    tarVector += n_gravity;
    foreach (var item in vectorEngineGroups)
    {
        int a = 0;
        double gAngle = tarVector.GetVectorAngle(out a, _gravity, false);
        //目标矢量与重力矢量夹角大于90度情况
        if (gAngle > Math.PI / 2)
        {
            Vector_2D _tarVector;
            double tVector;
            if (Math.Abs(Math.PI - gAngle) <= 0.05f)
            {
                item.SetMotorOrientation(n_gravity);
                item.SetThrusterThrust(0f);
            }
            else
            {
                if (a == 1)
                    _tarVector = VEHelp.Vector90Rotate(n_gravity, true);
                else
                    _tarVector = VEHelp.Vector90Rotate(n_gravity, false);
                item.SetMotorOrientation(_tarVector);
                tVector = (tarVector - n_gravity).Vector;
                if (item.isSuccess)
                    item.SetThrusterThrust((float)tVector, vectorEngineGroups.Count);
                else
                {
                    int rl;
                    double angle = item.ThrustVector.GetVectorAngle(out rl, n_gravity, false);
                    double scalar = (n_gravity.Vector / Math.Cos(angle));
                    item.SetThrusterThrust((float)scalar, vectorEngineGroups.Count);
                }
            }
        }
        else
        {
            item.SetMotorOrientation(tarVector);

            if (item.isSuccess)
                item.SetThrusterThrust((float)tarVector.Vector, vectorEngineGroups.Count);
            else
            {
                int rl;
                double angle = item.ThrustVector.GetVectorAngle(out rl, n_gravity, false);
                double scalar = (n_gravity.Vector / Math.Cos(angle));
                item.SetThrusterThrust((float)scalar, vectorEngineGroups.Count);
            }
        }
    }
}

//关闭引擎
public void ON_Off(bool para)
{
    foreach (var item in vectorEngineGroups)
        item.ThrustON_Off(para);
}
//转子复位
public void MotorReset(int y, int z)
{
    int int_y, int_z;
    if (y == 0 && z == 0)
    {
        int_y = MOTOR_DEFAULT[0];
        int_z = MOTOR_DEFAULT[1];
    }
    else
    {
        int_y = y;
        int_z = z;
    }
    foreach (var item in vectorEngineGroups)
        item.SetMotorOrientation(new Vector_2D(int_y, int_z));
}

//设置重力环境下的推力
public void Redistribution()
{
    float total = vectorEngineGroups.Sum(x => x.ThrustSum);
    Vector3D gravity = Vector3D.TransformNormal(ctrlBlock.GetNaturalGravity(), MatrixD.Transpose((ctrlBlock as IMyEntity).WorldMatrix));
    Vector_2D _gravity = new Vector_2D(gravity, 0.005f);
    float g = GetOvercomeGravity(_gravity);
    surplus_Thrust = (total - g) * 0.95f;
}

//构造函数
public Program()
{
    Runtime.UpdateFrequency = UpdateFrequency.Update1;
    //获取控制块
    List<IMyShipController> ctrls = new List<IMyShipController>();
    GridTerminalSystem.GetBlocksOfType(ctrls);
    ctrlBlock = ctrls.FirstOrDefault(x => x.CustomName.Contains(CONTROLER_BLOCK));
    //获取调试面板
    //Lcd = (IMyTextPanel)GridTerminalSystem.GetBlockWithName("Lcd");
    //获得编组并存入变量
    List<IMyBlockGroup> block_Groups = GetGroups(VECTOR_ENGINE_GROUP_NAME);
    if (block_Groups.Count > 0)
    {
        foreach (var bGroup in block_Groups)
        {
            string name = bGroup.Name;
            List<IMyMotorStator> motors = new List<IMyMotorStator>();
            List<IMyThrust> thrusts = new List<IMyThrust>();
            bGroup.GetBlocksOfType(motors);
            bGroup.GetBlocksOfType(thrusts);
            if (name.Contains(INSTALLATION_SITE_LEFT))
                vectorEngineGroups.Add(new VectorEngineGroup(motors[0], thrusts, VectorEngineGroup.LocationEnum.Left));
            else if (name.Contains(INSTALLATION_SITE_RIGTH))
                vectorEngineGroups.Add(new VectorEngineGroup(motors[0], thrusts, VectorEngineGroup.LocationEnum.Rigth));
        }
    }
    //设置重力环境下的推力
    Redistribution();
}
//入口函数
public void Main(string argument, UpdateType updateSource)
{
    if (!string.IsNullOrEmpty(argument))
        exec_para = argument;
    string execModel = string.Empty;
    switch (exec_para)
    {
        case "1":
            execModel = "无重力环境";
            if (oldExec != exec_para)
                ON_Off(true);
            AgravicModel();
            break;
        case "2":
            execModel = "有重力环境";
            if (oldExec != exec_para)
                ON_Off(true);
            gravityModel();
            break;
        case "r":
            execModel = "重新分配推力";
            if (oldExec != exec_para)
                Redistribution();
            break;
        default:
            if (exec_para.Contains("off"))
            {
                if (oldExec != exec_para)
                    ON_Off(false);
                int[] ints = new int[] { 0, 0 };
                try
                {
                    string data = exec_para.Split(' ')[1];
                    string[] values = data.Split(',');
                    bool valit = int.TryParse(values[0], out ints[0]) && int.TryParse(values[1], out ints[1]);
                    if (!valit) { throw new Exception(); } else { execModel = "关闭并复位"; }
                }
                catch (Exception)
                {
                    execModel = "关闭并复位,复位数值无效,复位至预设角度";
                }
                finally
                {
                    MotorReset(ints[0], ints[1]);
                }
            }
            else { execModel = "无效运行输入"; }
            break;
    }
    oldExec = exec_para;
    Echo($"正常运行_当前为:{execModel}");
}
#endregion