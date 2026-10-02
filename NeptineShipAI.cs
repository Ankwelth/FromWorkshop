/*
* Ship AI System 
* Built by mlq1616
* https://github.com/mlq1819
*/
//Name me!
const string Program_Name="Neptine Ship AI"; 
//The angle of what the ship will accept as "correct"
const double ACCEPTABLE_ANGLE=10; //Suggested between 5° and 20°
//The maximum speed limit of the ship
const double SPEED_LIMIT=100;
//The distance accepted for raycasts
const double RAYCAST_DISTANCE=5000; //The lower the better, but whatever works for you
//Set this to the distance you want lights, sound blocks, and doors to update when enemies are nearby
const double ALERT_DISTANCE=15;
//Set this to a string contained in the name of lights you want to change the color of during a lockdown
const string LOCKDOWN_LIGHT_NAME="";
//Set this to a string contained in the name of doors you want to automatically open when a player gets near
const string AUTODOOR_NAME="AutoDoor";
Color DEFAULT_TEXT_COLOR=new Color(197,137,255,255);
Color DEFAULT_BACKGROUND_COLOR=new Color(44,0,88,255);
Color LOCKDOWN_COLOR=new Color(255,255,125,255);

public class GenericMethods<T> where T : class, IMyTerminalBlock{
	IMyGridTerminalSystem TerminalSystem;
	IMyTerminalBlock Prog;
	MyGridProgram Program;
	
	public GenericMethods(MyGridProgram Program){
		this.Program=Program;
		TerminalSystem=Program.GridTerminalSystem;
		Prog=Program.Me;
	}
	
	public T GetFull(string name, double max_distance, Vector3D Reference){
		List<T> AllBlocks=new List<T>();
		List<T> MyBlocks=new List<T>();
		TerminalSystem.GetBlocksOfType<T>(AllBlocks);
		double min_distance=max_distance;
		foreach(T Block in AllBlocks){
			if(Block.CustomName.Equals(name)){
				double distance=(Reference-Block.GetPosition()).Length();
				min_distance=Math.Min(min_distance, distance);
				MyBlocks.Add(Block);
			}
		}
		foreach(T Block in MyBlocks){
			double distance=(Reference-Block.GetPosition()).Length();
			if(distance<=min_distance+0.1){
				return Block;
			}
		}
		return null;
	}
	
	public T GetFull(string name, double max_distance, IMyTerminalBlock Reference){
		return GetFull(name, max_distance, Reference.GetPosition());
	}
	
	public T GetFull(string name, double max_distance){
		return GetFull(name, max_distance, Prog);
	}
	
	public T GetFull(string name){
		return GetFull(name, double.MaxValue);
	}
	
	public List<T> GetAllConstruct(string name, double max_distance, IMyTerminalBlock Reference){
		List<T> input=GetAllContaining(name,max_distance,Reference);
		List<T> output=new List<T>();
		foreach(T Block in input){
			if(Reference.IsSameConstructAs(Block))
				output.Add(Block);
		}
		return output;
	}
	
	public List<T> GetAllConstruct(string name, IMyTerminalBlock Reference){
		return GetAllConstruct(name,double.MaxValue,Reference);
	}
	
	public List<T> GetAllConstruct(string name){
		return GetAllConstruct(name,Prog);
	}
	
	public T GetContaining(string name, double max_distance, Vector3D Reference){
		List<T> AllBlocks=new List<T>();
		List<T> MyBlocks=new List<T>();
		TerminalSystem.GetBlocksOfType<T>(AllBlocks);
		double min_distance=max_distance;
		foreach(T Block in AllBlocks){
			if(Block.CustomName.Contains(name)){
				double distance=(Reference-Block.GetPosition()).Length();
				min_distance=Math.Min(min_distance, distance);
				MyBlocks.Add(Block);
			}
		}
		foreach(T Block in MyBlocks){
			double distance=(Reference-Block.GetPosition()).Length();
			if(distance<=min_distance+0.1){
				return Block;
			}
		}
		return null;
	}
	
	public T GetContaining(string name, double max_distance, IMyTerminalBlock Reference){
		return GetContaining(name, max_distance, Reference.GetPosition());
	}
	
	public T GetContaining(string name, double max_distance){
		return GetContaining(name, max_distance, Prog);
	}
	
	public T GetContaining(string name){
		return GetContaining(name, double.MaxValue);
	}
	
	public List<T> GetAllIncluding(string name, Vector3D Reference, double max_distance=double.MaxValue){
		List<T> AllBlocks=new List<T>();
		List<T> MyBlocks=new List<T>();
		TerminalSystem.GetBlocksOfType<T>(AllBlocks);
		foreach(T Block in AllBlocks){
			double distance=(Reference-Block.GetPosition()).Length();
			if(Block.CustomName.Contains(name) && distance <= max_distance){
				MyBlocks.Add(Block);
			}
		}
		return MyBlocks;
	}
	
	public List<T> GetAllIncluding(string name, IMyTerminalBlock Reference, double max_distance=double.MaxValue){
		return GetAllIncluding(name, Reference.GetPosition(), max_distance);
	}
	
	public List<T> GetAllIncluding(string name, double max_distance=double.MaxValue){
		return GetAllIncluding(name, Prog, max_distance);
	}
	
	public List<T> GetAllContaining(string name, double max_distance, Vector3D Reference){
		List<T> AllBlocks=new List<T>();
		List<List<T>> MyLists=new List<List<T>>();
		List<T> MyBlocks=new List<T>();
		TerminalSystem.GetBlocksOfType<T>(AllBlocks);
		foreach(T Block in AllBlocks){
			if(Block.CustomName.Contains(name)){
				bool has_with_name=false;
				for(int i=0; i<MyLists.Count && !has_with_name; i++){
					if(Block.CustomName.Equals(MyLists[i][0].CustomName)){
						MyLists[i].Add(Block);
						has_with_name=true;
						break;
					}
				}
				if(!has_with_name){
					List<T> new_list=new List<T>();
					new_list.Add(Block);
					MyLists.Add(new_list);
				}
			}
		}
		foreach(List<T> list in MyLists){
			if(list.Count==1){
				MyBlocks.Add(list[0]);
				continue;
			}
			double min_distance=max_distance;
			foreach(T Block in list){
				double distance=(Reference-Block.GetPosition()).Length();
				min_distance=Math.Min(min_distance, distance);
			}
			foreach(T Block in list){
				double distance=(Reference-Block.GetPosition()).Length();
				if(distance<=min_distance+0.1){
					MyBlocks.Add(Block);
					break;
				}
			}
		}
		return MyBlocks;
	}
	
	public List<T> GetAllContaining(string name, double max_distance, IMyTerminalBlock Reference){
		return GetAllContaining(name, max_distance, Reference.GetPosition());
	}
	
	public List<T> GetAllContaining(string name, double max_distance){
		return GetAllContaining(name, max_distance, Prog);
	}
	
	public List<T> GetAllContaining(string name){
		return GetAllContaining(name, double.MaxValue);
	}
	
	public List<T> GetAllFunc(Func<T, bool> f){
		List<T> AllBlocks=new List<T>();
		List<T> MyBlocks=new List<T>();
		TerminalSystem.GetBlocksOfType<T>(AllBlocks);
		foreach(T Block in AllBlocks){
			if(f(Block)){
				MyBlocks.Add(Block);
			}
		}
		return MyBlocks;
	}
	
	public T GetClosestFunc(Func<T, bool> f, double max_distance, Vector3D Reference){
		List<T> MyBlocks=GetAllFunc(f);
		double min_distance=max_distance;
		foreach(T Block in MyBlocks){
			double distance=(Reference-Block.GetPosition()).Length();
			min_distance=Math.Min(min_distance, distance);
		}
		foreach(T Block in MyBlocks){
			double distance=(Reference-Block.GetPosition()).Length();
			if(distance<=min_distance+0.1){
				return Block;
			}
		}
		return null;
	}
	
	public T GetClosestFunc(Func<T, bool> f, double max_distance, IMyTerminalBlock Reference){
		return GetClosestFunc(f, max_distance, Reference.GetPosition());
	}
	
	public T GetClosestFunc(Func<T, bool> f, double max_distance){
		return GetClosestFunc(f, max_distance, Program.Me);
	}
	
	public T GetClosestFunc(Func<T, bool> f){
		return GetClosestFunc(f, double.MaxValue);
	}
	
	public T GetGrid(string name, IMyCubeGrid Grid, double max_distance, IMyTerminalBlock Reference){
		List<T> input=GetAllGrid(name,Grid,max_distance,Reference);
		if(input.Count>0)
			return input[0];
		return null;
	}
	
	public T GetGrid(string name,IMyCubeGrid Grid,double max_distance=double.MaxValue){
		return GetGrid(name,Grid,max_distance,Prog);
	}
	
	public List<T> GetAllGrid(string name, IMyCubeGrid Grid, double max_distance,IMyTerminalBlock Reference){
		List<T> output=new List<T>();
		List<T> input=GetAllContaining(name,max_distance,Reference);
		foreach(T Block in input){
			if(Block.CubeGrid==Grid)
				output.Add(Block);
		}
		return output;
	}
	
	public List<T> GetAllGrid(string name, IMyCubeGrid Grid, double max_distance=double.MaxValue){
		return GetAllGrid(name,Grid,max_distance,Prog);
	}
	
	public static List<T> SortByDistance(List<T> unsorted, Vector3D Reference){
		List<T> output=new List<T>();
		while(unsorted.Count > 0){
			double min_distance=double.MaxValue;
			foreach(T Block in unsorted){
				double distance=(Reference-Block.GetPosition()).Length();
				min_distance=Math.Min(min_distance, distance);
			}
			for(int i=0; i<unsorted.Count; i++){
				double distance=(Reference-unsorted[i].GetPosition()).Length();
				if(distance<=min_distance+0.1){
					output.Add(unsorted[i]);
					unsorted.RemoveAt(i);
					break;
				}
			}
		}
		return output;
	}
	
	public static List<T> SortByDistance(List<T> unsorted, IMyTerminalBlock Reference){
		return SortByDistance(unsorted, Reference.GetPosition());
	}
	
	public List<T> SortByDistance(List<T> unsorted){
		return SortByDistance(unsorted, Prog);
	}
	
	public static double GetAngle(Vector3D v1, Vector3D v2){
		v1.Normalize();
		v2.Normalize();
		return Math.Round(Math.Acos(v1.X*v2.X+v1.Y*v2.Y+v1.Z*v2.Z)*57.295755, 5);
	}
}

public class EntityInfo{
	public long ID;
	public string Name;
	public MyDetectedEntityType Type;
	Vector3D? _hitposition;
	public Vector3D? HitPosition{
		get{
			return _hitposition;
		}
		set{
			_hitposition=value;
			if(_hitposition!=null){
				Size=Math.Max(Size, (Position-((Vector3D)_hitposition)).Length());
			}
		}
	}
	Vector3D _velocity;
	public Vector3D Velocity{
		get{
			return _velocity;
		}
		set{
			_velocity=value;
			Age=TimeSpan.Zero;
		}
	}
	public MyRelationsBetweenPlayerAndBlock Relationship;
	public Vector3D Position;
	public double Size=0;
	public TimeSpan Age=TimeSpan.Zero;
	
	public EntityInfo(long id, string name, MyDetectedEntityType type, Vector3D? hitposition, Vector3D velocity, MyRelationsBetweenPlayerAndBlock relationship, Vector3D position){
		ID=id;
		Name=name;
		Type=type;
		HitPosition=hitposition;
		Velocity=velocity;
		Relationship=relationship;
		Position=position;
		Age=TimeSpan.Zero;
	}
	
	public EntityInfo(long id, string name, MyDetectedEntityType type, Vector3D? hitposition, Vector3D velocity, MyRelationsBetweenPlayerAndBlock relationship, Vector3D position, double size) : this(id, name, type, hitposition, velocity, relationship, position){
		this.Size=size;
	}
	
	public EntityInfo(EntityInfo o){
		ID=o.ID;
		Name=o.Name;
		Type=o.Type;
		Position=o.Position;
		HitPosition=o.HitPosition;
		Velocity=o.Velocity;
		Relationship=o.Relationship;
		Size=o.Size;
		Age=o.Age;
	}
	
	public EntityInfo(MyDetectedEntityInfo entity_info){
		ID=entity_info.EntityId;
		Name=entity_info.Name;
		Type=entity_info.Type;
		Position=entity_info.Position;
		if(entity_info.HitPosition!=null){
			HitPosition=entity_info.HitPosition;
		}
		else {
			HitPosition=(Vector3D?) null;
		}
		Velocity=entity_info.Velocity;
		Relationship=entity_info.Relationship;
		Age=TimeSpan.Zero;
	}
	
	public static bool TryParse(string Parse, out EntityInfo Entity){
		Entity=new EntityInfo(-1,"bad", MyDetectedEntityType.None, null, new Vector3D(0,0,0), MyRelationsBetweenPlayerAndBlock.NoOwnership, new Vector3D(0,0,0));
		try{
			string[] args=Parse.Split('\n');
			long id;
			if(!Int64.TryParse(args[0], out id)){
				return false;
			}
			string name=args[1];
			MyDetectedEntityType type=(MyDetectedEntityType) Int32.Parse(args[2]);
			Vector3D? hitposition;
			if(args[3].Equals("null")){
				hitposition=(Vector3D?) null;
			}
			else {
				Vector3D temp;
				if(!Vector3D.TryParse(args[3], out temp)){
					return false;
				}
				else {
					hitposition=(Vector3D?) temp;
				}
			}
			Vector3D velocity;
			if(!Vector3D.TryParse(args[4], out velocity)){
				return false;
			}
			MyRelationsBetweenPlayerAndBlock relationship=(MyRelationsBetweenPlayerAndBlock) Int32.Parse(args[5]);
			Vector3D position;
			if(!Vector3D.TryParse(args[6], out position)){
				return false;
			}
			double size=0;
			if(!double.TryParse(args[7], out size)){
				size=0;
			}
			TimeSpan age;
			if(!TimeSpan.TryParse(args[8], out age)){
				return false;
			}
			Entity=new EntityInfo(id, name, type, hitposition, velocity, relationship, position, size);
			Entity.Age=age;
			return true;
		}
		catch(Exception){
			return false;
		}
	}
	
	public override string ToString(){
		string entity_info="";
		entity_info+=ID.ToString()+'\n';
		entity_info+=Name.ToString()+'\n';
		entity_info+=((int)Type).ToString()+'\n';
		if(HitPosition!=null){
			entity_info+=((Vector3D) HitPosition).ToString()+'\n';
		}
		else {
			entity_info+="null"+'\n';
		}
		entity_info+=Velocity.ToString()+'\n';
		entity_info+=((int)Relationship).ToString()+'\n';
		entity_info+=Position.ToString()+'\n';
		entity_info+=Size.ToString()+'\n';
		entity_info+=Age.ToString()+'\n';
		return entity_info;
	}
	
	public string NiceString(){
		string entity_info="";
		entity_info+="ID: "+ID.ToString()+'\n';
		entity_info+="Name: "+Name.ToString()+'\n';
		entity_info+="Type: "+Type.ToString()+'\n';
		if(HitPosition!=null){
			entity_info+="HitPosition: "+NeatVector((Vector3D) HitPosition)+'\n';
		}
		else {
			entity_info+="HitPosition: "+"null"+'\n';
		}
		entity_info+="Velocity: "+NeatVector(Velocity)+'\n';
		entity_info+="Relationship: "+Relationship.ToString()+'\n';
		entity_info+="Position: "+NeatVector(Position)+'\n';
		entity_info+="Size: "+((int)Size).ToString()+'\n';
		entity_info+="Data Age: "+Age.ToString()+'\n';
		return entity_info;
	}
	
	public static string NeatVector(Vector3D vector){
		return "X:"+Math.Round(vector.X,1).ToString()+" Y:"+Math.Round(vector.Y,1).ToString()+" Z:"+Math.Round(vector.Z,1).ToString();
	}
	
	public void Update(double seconds){
		TimeSpan time=new TimeSpan((int)(seconds/60/60/24), ((int)(seconds/60/60))%24, ((int)(seconds/60))%60, ((int)(seconds))%60, ((int)(seconds*1000))%1000);
		Age.Add(time);
		Position+=seconds*Velocity;
		if(HitPosition!=null){
			HitPosition=(Vector3D?) (((Vector3D)HitPosition)+seconds*Velocity);
		}
	}
	
	public double GetDistance(Vector3D Reference){
		return (Position-Reference).Length();
	}
}

public class EntityList : IEnumerable<EntityInfo>{
	List<EntityInfo> E_List;
	public IEnumerator<EntityInfo> GetEnumerator(){
		return E_List.GetEnumerator();
	}
	IEnumerator IEnumerable.GetEnumerator(){
		return GetEnumerator();
	}
	
	public int Count{
		get{
			return E_List.Count;
		}
	}
	
	public EntityInfo this[int key]{
		get{
			return E_List[key];
		}
		set{
			E_List[key]=value;
		}
	}
	
	public EntityList(){
		E_List=new List<EntityInfo>();
	}
	
	public void UpdatePositions(double seconds){
		foreach(EntityInfo entity in E_List){
			entity.Update(seconds);
		}
	}
	
	public bool UpdateEntry(EntityInfo Entity){
		for(int i=0; i<E_List.Count; i++){
			if(E_List[i].ID==Entity.ID || (Entity.GetDistance(E_List[i].Position)<=0.5f&&Entity.Type==E_List[i].Type)){
				if(E_List[i].Age >= Entity.Age){
					E_List[i]=Entity;
					return true;
				}
				return false;
			}
		}
		E_List.Add(Entity);
		return true;
	}
	
	public bool RemoveEntry(EntityInfo Entity){
		for(int i=0; i<E_List.Count; i++){
			if(E_List[i].ID==Entity.ID || (Entity.GetDistance(E_List[i].Position)<=0.5f&&Entity.Type==E_List[i].Type)){
				E_List.RemoveAt(i);
				return true;
			}
		}
		return false;
	}
	
	public EntityInfo Get(long ID){
		foreach(EntityInfo entity in E_List){
			if(entity.ID==ID)
				return entity;
		}
		return null;
	}
	
	public double ClosestDistance(MyGridProgram P, MyRelationsBetweenPlayerAndBlock Relationship, double min_size=0){
		double min_distance=double.MaxValue;
		foreach(EntityInfo entity in E_List){
			if(entity.Size >= min_size && entity.Relationship==Relationship){
				min_distance=Math.Min(min_distance, (P.Me.GetPosition()-entity.Position).Length()-entity.Size);
			}
		}
		return min_distance;
	}
	
	public double ClosestDistance(MyGridProgram P, double min_size=0){
		double min_distance=double.MaxValue;
		foreach(EntityInfo entity in E_List){
			if(entity.Size >= min_size){
				min_distance=Math.Min(min_distance, (P.Me.GetPosition()-entity.Position).Length()-entity.Size);
			}
		}
		return min_distance;
	}
	
	public void Clear(){
		E_List.Clear();
	}
	
	public void Sort(Vector3D Reference){
		List<EntityInfo> Sorted=new List<EntityInfo>();
		List<EntityInfo> Unsorted=new List<EntityInfo>();
		foreach(EntityInfo Entity in E_List){
			double distance=Entity.GetDistance(Reference);
			double last_distance=0;
			if(Sorted.Count>0)
				last_distance=Sorted[Sorted.Count-1].GetDistance(Reference);
			if(distance>=last_distance)
				Sorted.Add(Entity);
			else
				Unsorted.Add(Entity);
		}
		while(Unsorted.Count>0){
			double distance=Unsorted[0].GetDistance(Reference);
			if(distance>=Sorted[Sorted.Count-1].GetDistance(Reference)){
				Sorted.Add(Unsorted[0]);
				Unsorted.RemoveAt(0);
				continue;
			}
			for(int i=0;i<Sorted.Count;i++){
				if(distance<=Sorted[i].GetDistance(Reference)){
					Sorted.Insert(i,Unsorted[0]);
					Unsorted.RemoveAt(0);
					break;
				}
			}
		}
		E_List=Sorted;
	}
}

public enum MenuType{
	Submenu=0,
	Command=1,
	Display=2
}

public interface MenuOption{
	string Name();
	MenuType Type();
	bool AutoRefresh();
	int Depth();
	bool Back();
	bool Select();
}

public class Menu_Submenu : MenuOption{
	string _Name;
	public string Name(){
		return _Name;
	}
	public MenuType Type(){
		return MenuType.Submenu;
	}
	public bool AutoRefresh(){
		if(IsSelected){
			return Menu[Selection].AutoRefresh();
		}
		return Last_Count==Count || Count>10;
	}
	public int Depth(){
		if(Selected){
			return 1+Menu[Selection].Depth();
		}
		return 1;
	}
	bool Selected;
	public bool IsSelected{
		get{
			return Selected;
		}
	}
	int Selection;
	
	int Last_Count;
	public int Count{
		get{
			return Menu.Count;
		}
	}
	
	List<MenuOption> Menu;
	
	public Menu_Submenu(string name){
		_Name=name.Trim().Substring(0, Math.Min(name.Trim().Length, 24));
		Menu=new List<MenuOption>();
		Selection=0;
		Last_Count=0;
	}
	
	public bool Add(MenuOption new_item){
		Menu.Add(new_item);
		return true;
	}
	
	public bool Back(){
		if(Selected){
			if(Menu[Selection].Back())
				return true;
			Selected=false;
			return true;
		}
		return false;
	}
	
	public bool Select(){
		if(Selected){
			bool output=Menu[Selection].Select();
			if(Menu[Selection].Type()==MenuType.Command)
				Selected=false;
			return output;
		}
		Selected=true;
		return true;
	}
	
	public bool Next(){
		if(Selected){
			if(Menu[Selection].Type()==MenuType.Submenu){
				return ((Menu_Submenu)(Menu[Selection])).Next();
			}
			return false;
		}
		if(Count>0)
			Selection=(Selection+1)%Count;
		return true;
	}
	
	public bool Prev(){
		if(Selected){
			if(Menu[Selection].Type()==MenuType.Submenu){
				return ((Menu_Submenu)(Menu[Selection])).Prev();
			}
			return false;
		}
		if(Count>0)
			Selection=(Selection-1+Count)%Count;
		return true;
	}
	
	public bool Replace(Menu_Submenu Replacement){
		for(int i=0;i<Count;i++){
			if(Menu[i].Name().Equals(Replacement.Name())){
				Menu[i]=Replacement;
				return true;
			}
		}
		return false;
	}
	
	public override string ToString(){
		if(Count>0)
			Selection=Selection%Count;
		if(Selected){
			return Menu[Selection].ToString();
		}
		string output=" -- "+Name()+" -- ";
		if(Count <= 10){
			for(int i=0; i<Count; i++){
				output+="\n ";
				switch(Menu[i].Type()){
					case MenuType.Submenu:
						output+="[";
						break;
					case MenuType.Command:
						output+="<";
						break;
					case MenuType.Display:
						output+="(";
						break;
				}
				output+=' ';
				if(Selection==i){
					output+=' '+Menu[i].Name().ToUpper()+' ';
				}
				else {
					output+=Menu[i].Name().ToLower();
				}
				switch(Menu[i].Type()){
					case MenuType.Submenu:
						output+=" ("+(Menu[i] as Menu_Submenu).Count.ToString()+")]";
						break;
					case MenuType.Command:
						output+=">";
						break;
					case MenuType.Display:
						output+=")";
						break;
				}
			}
		}
		else {
			int count=0;
			for(int i=Selection; count<=10 && i<Count;i++){
				count++;
				output+="\n ";
				switch(Menu[i].Type()){
					case MenuType.Submenu:
						output+="[";
						break;
					case MenuType.Command:
						output+="<";
						break;
					case MenuType.Display:
						output+="(";
						break;
				}
				output+=' ';
				if(Selection==i){
					output+=' '+Menu[i].Name().ToUpper()+' ';
				}
				else {
					output+=Menu[i].Name().ToLower();
				}
				switch(Menu[i].Type()){
					case MenuType.Submenu:
						output+="]";
						break;
					case MenuType.Command:
						output+=">";
						break;
					case MenuType.Display:
						output+=")";
						break;
				}
			}
		}
		return output;
	}
}

public class Menu_Command<T> : MenuOption where T : class{
	string _Name;
	public string Name(){
		return _Name;
	}
	public MenuType Type(){
		return MenuType.Command;
	}
	bool _AutoRefresh;
	public bool AutoRefresh(){
		return _AutoRefresh;
	}
	public int Depth(){
		return 1;
	}
	string Desc;
	T Arg;
	Func<T, bool> Command;
	
	public Menu_Command(string name, Func<T, bool> command, string desc="No description provided", T arg=null, bool autorefresh=false){
		if(name.Trim().Length > 0)
			_Name=name;
		Desc=desc;
		Arg=arg;
		Command=command;
		_AutoRefresh=autorefresh;
	}
	
	public bool Select(){
		return Command(Arg);
	}
	
	public bool Back(){
		return false;
	}
	
	public override string ToString(){
		string output=Name()+'\n';
		string[] words=Desc.Split(' ');
		int length=24;
		foreach(string word in words){
			if(length > 0 && length+word.Length > 24){
				length=0;
				output+='\n';
			}
			else {
				output+=' ';
			}
			output+=word;
			if(word.Contains('\n'))
				length=word.Length-word.IndexOf('\n')-1;
			else
				length+=word.Length;
		}
		return output+"\n\nSelect to Execute";
	}
}

public class Menu_Display : MenuOption{
	public string Name(){
		if(Entity==null){
			return "null";
		}
		string name=Entity.Name.Substring(0, Math.Min(24, Entity.Name.Length));
		string[] args=name.Split(' ');
		int number=0;
		if(args.Length==3 && args[1].ToLower().Equals("grid") && Int32.TryParse(args[2], out number)){
			name="Unnamed "+args[0]+' '+args[1];
		}
		double distance=Entity.GetDistance(P.Me.GetPosition())-Entity.Size;
		string distance_string=Math.Round(distance,0).ToString()+"M";
		if(distance>=1000)
			distance_string=Math.Round(distance/1000,1).ToString()+"kM";
		string output=' '+name+' '+distance_string;
		switch(Entity.Relationship){
			case MyRelationsBetweenPlayerAndBlock.Owner:
				return ''+output;
			case MyRelationsBetweenPlayerAndBlock.FactionShare:
				return ''+output;
			case MyRelationsBetweenPlayerAndBlock.Friends:
				return ''+output;
			case MyRelationsBetweenPlayerAndBlock.NoOwnership:
				return ''+output;
			case MyRelationsBetweenPlayerAndBlock.Enemies:
				return ''+output;
		}
		return ''+output;
	}
	public MenuType Type(){
		return MenuType.Display;
	}
	public bool AutoRefresh(){
		return true;
	}
	public int Depth(){
		if(Selected)
			return 2;
		return 1;
	}
	bool Selected;
	EntityInfo Entity;
	bool Can_GoTo;
	Func<EntityInfo, bool> Command;
	Menu_Command<EntityInfo> Subcommand {
		get{
			double distance=(P.Me.GetPosition()-Entity.Position).Length()-Entity.Size;
			return new Menu_Command<EntityInfo>("GoTo "+Entity.Name, Command, "Set autopilot to match target Entity's expected position and velocity", Entity);
		}
	}
	MyGridProgram P;
	
	public Menu_Display(EntityInfo entity, MyGridProgram p, Func<EntityInfo, bool> GoTo){
		P=p;
		Entity=entity;
		Command=GoTo;
		Selected=false;
		Can_GoTo=true;
	}
	
	public Menu_Display(EntityInfo entity, MyGridProgram p){
		Entity=entity;
		P=p;
		Selected=false;
		Can_GoTo=false;
	}
	
	public bool Select(){
		if(!Can_GoTo)
			return false;
		if(Selected){
			if(Command==null)
				return false;
			if(Subcommand.Select()){
				Selected=false;
				return true;
			}
			return false;
		}
		Selected=true;
		return true;
	}
	
	public bool Back(){
		if(!Selected){
			return false;
		}
		Selected=false;
		return true;
	}
	
	public override string ToString(){
		if(Selected){
			return Subcommand.ToString();
		}
		else {
			double distance=Entity.GetDistance(P.Me.GetPosition());
			string distance_string=Math.Round(distance,0)+"M";
			if(distance>=1000)
				distance_string=Math.Round(distance/1000,1)+"kM";
			return Entity.NiceString()+"Distance: "+distance_string;
		}
	}
}

struct Airlock{
	public IMyDoor Door1;
	public IMyDoor Door2;
	public Airlock(IMyDoor d1, IMyDoor d2){
		Door1=d1;
		Door2=d2;
	}
	public bool Equals(Airlock o){
		return Door1.Equals(o.Door1) && Door2.Equals(o.Door2);
	}
	public double Distance(Vector3D Reference){
		double distance_1=(Reference-Door1.GetPosition()).Length();
		double distance_2=(Reference-Door2.GetPosition()).Length();
		return Math.Min(distance_1, distance_2);
	}
}

long cycle_long=1;
long cycle=0;
char loading_char='|';
double seconds_since_last_update=0;
Random Rnd;

IMyShipController Controller;
IMyGyro Gyroscope;

List<IMyTextPanel> StatusLCDs;
List<IMyTextPanel> DebugLCDs;

List<IMyDoor> AutoDoors;

List<Airlock> Airlocks;

EntityList AsteroidList;
EntityList PlanetList;
EntityList SmallShipList;
EntityList LargeShipList;
EntityList CharacterList;

List<IMyThrust> Forward_Thrusters;
List<IMyThrust> Backward_Thrusters;
List<IMyThrust> Up_Thrusters;
List<IMyThrust> Down_Thrusters;
List<IMyThrust> Left_Thrusters;
List<IMyThrust> Right_Thrusters;
float Forward_Thrust{
	get{
		float total=0;
		foreach(IMyThrust Thruster in Forward_Thrusters)
			total+=Thruster.MaxEffectiveThrust;
		return total;
	}
}
float Backward_Thrust{
	get{
		float total=0;
		foreach(IMyThrust Thruster in Backward_Thrusters)
			total+=Thruster.MaxEffectiveThrust;
		return total;
	}
}
float Up_Thrust{
	get{
		float total=0;
		foreach(IMyThrust Thruster in Up_Thrusters)
			total+=Thruster.MaxEffectiveThrust;
		return total;
	}
}
float Down_Thrust{
	get{
		float total=0;
		foreach(IMyThrust Thruster in Down_Thrusters)
			total+=Thruster.MaxEffectiveThrust;
		return total;
	}
}
float Left_Thrust{
	get{
		float total=0;
		foreach(IMyThrust Thruster in Left_Thrusters)
			total+=Thruster.MaxEffectiveThrust;
		return total;
	}
}
float Right_Thrust{
	get{
		float total=0;
		foreach(IMyThrust Thruster in Right_Thrusters)
			total+=Thruster.MaxEffectiveThrust;
		return total;
	}
}

double Time_To_Crash=double.MaxValue;
Menu_Submenu Command_Menu;

Base6Directions.Direction Forward;
Base6Directions.Direction Backward{
	get{
		return Base6Directions.GetOppositeDirection(Forward);
	}
}
Base6Directions.Direction Up;
Base6Directions.Direction Down{
	get{
		return Base6Directions.GetOppositeDirection(Up);
	}
}
Base6Directions.Direction Left;
Base6Directions.Direction Right{
	get{
		return Base6Directions.GetOppositeDirection(Left);
	}
}

Vector3D Forward_Vector;
Vector3D Backward_Vector{
	get{
		return -1*Forward_Vector;
	}
}
Vector3D Up_Vector;
Vector3D Down_Vector{
	get{
		return -1*Up_Vector;
	}
}
Vector3D Left_Vector;
Vector3D Right_Vector{
	get{
		return -1*Left_Vector;
	}
}

bool Tracking=false;
bool Match_Direction=false;
Vector3D Target_Direction;
bool Match_Position=false;
Vector3D Target_Position;
Vector3D Relative_Target_Position{
	get{
		return GlobalToLocalPosition(Target_Position);
	}
}
double Target_Distance{
	get{
		return (Target_Position-Controller.GetPosition()).Length();
	}
}
long Target_ID=0;

float Mass_Accomodation=0.0f;

Vector3D RestingVelocity;
Vector3D Relative_RestingVelocity{
	get{
		return GlobalToLocal(RestingVelocity);
	}
}
Vector3D CurrentVelocity;
Vector3D Relative_CurrentVelocity{
	get{
		Vector3D output=Vector3D.Transform(CurrentVelocity+Controller.GetPosition(), MatrixD.Invert(Controller.WorldMatrix));
		output.Normalize();
		output *= CurrentVelocity.Length();
		return output;
	}
}
Vector3D Gravity;
Vector3D Relative_Gravity{
	get{
		return GlobalToLocal(Gravity);
	}
}
Vector3D Adjusted_Gravity{
	get{
		Vector3D temp=GlobalToLocal(Gravity);
		temp.Normalize();
		return temp*Mass_Accomodation;
	}
}
Vector3D Gravity_Direction{
	get{
		Vector3D direction=Gravity;
		direction.Normalize();
		return direction;
	}
}

double Speed_Deviation{
	get{
		return (CurrentVelocity-RestingVelocity).Length();
	}
}

Vector3D AngularVelocity;
Vector3D Relative_AngularVelocity{
	get{
		return GlobalToLocal(AngularVelocity);
	}
}

double Elevation;
double Sealevel;
Vector3D PlanetCenter;

bool HasBlockData(IMyTerminalBlock Block, string Name){
	if(Name.Contains(':'))
		return false;
	string[] args=Block.CustomData.Split('•');
	foreach(string argument in args){
		if(argument.IndexOf(Name+':')==0){
			return true;
		}
	}
	return false;
}

string GetBlockData(IMyTerminalBlock Block, string Name){
	if(Name.Contains(':'))
		return "";
	string[] args=Block.CustomData.Split('•');
	foreach(string argument in args){
		if(argument.IndexOf(Name+':')==0){
			return argument.Substring((Name+':').Length);
		}
	}
	return "";
}

bool SetBlockData(IMyTerminalBlock Block, string Name, string Data){
	if(Name.Contains(':'))
		return false;
	string[] args=Block.CustomData.Split('•');
	for(int i=0; i<args.Count(); i++){
		if(args[i].IndexOf(Name+':')==0){
			Block.CustomData=Name+':'+Data;
			for(int j=0; j<args.Count(); j++){
				if(j!=i){
					Block.CustomData+='•'+args[j];
				}
			}
			return true;
		}
	}
	Block.CustomData+='•'+Name+':'+Data;
	return true;
}

bool CanHaveJob(IMyTerminalBlock Block, string JobName){
	return (!HasBlockData(Block, "Job")) || GetBlockData(Block, "Job").Equals("None") || GetBlockData(Block, "Job").Equals(JobName);
}

public void Write(string text, bool new_line=true, bool append=true){
	Echo(text);
	if(new_line)
		Me.GetSurface(0).WriteText(text+'\n', append);
	else
		Me.GetSurface(0).WriteText(text, append);
	foreach(IMyTextPanel Display in DebugLCDs){
		if(new_line)
			Display.WriteText(text+'\n', append);
		else
			Display.WriteText(text, append);
	}
}

public void Reset(){
	Write("Resetting");
	Operational=false;
	Controller=null;
	Gyroscope=null;
	AsteroidList=new EntityList();
	PlanetList=new EntityList();
	SmallShipList=new EntityList();
	LargeShipList=new EntityList();
	CharacterList=new EntityList();
	StatusLCDs=new List<IMyTextPanel>();
	DebugLCDs=new List<IMyTextPanel>();
	List<Airlock> Airlocks=new List<Airlock>();
	AutoDoors=new List<IMyDoor>();
	Forward_Thrusters=new List<IMyThrust>();
	Backward_Thrusters=new List<IMyThrust>();
	Up_Thrusters=new List<IMyThrust>();
	Down_Thrusters=new List<IMyThrust>();
	Left_Thrusters=new List<IMyThrust>();
	Right_Thrusters=new List<IMyThrust>();
	RestingVelocity=new Vector3D(0,0,0);
	Runtime.UpdateFrequency=UpdateFrequency.None;
}

bool ControllerFunction(IMyShipController ctrlr){
	IMyRemoteControl Remote=ctrlr as IMyRemoteControl;
	if(Remote!=null)
		return ctrlr.ControlThrusters;
	else
		return (ctrlr.ControlThrusters && ctrlr.IsMainCockpit);
}

void SetupAirlocks(){
	Airlocks=new List<Airlock>();
	List<IMyDoor> AllAirlockDoors=(new GenericMethods<IMyDoor>(this)).GetAllConstruct("Airlock");
	List<IMyDoor> AllAirlockDoor1s=new List<IMyDoor>();
	List<IMyDoor> AllAirlockDoor2s=new List<IMyDoor>();
	foreach(IMyDoor Door in AllAirlockDoors){
		if(Door.CustomName.Contains("Door 1")){
			AllAirlockDoor1s.Add(Door);
		}
		else if(Door.CustomName.Contains("Door 2")){
			AllAirlockDoor2s.Add(Door);
		}
	}
	List<List<IMyDoor>> PossibleAirlockDoor1Pairs=new List<List<IMyDoor>>();
	foreach(IMyDoor Door1 in AllAirlockDoor1s){
		List<IMyDoor> pair=new List<IMyDoor>();
		pair.Add(Door1);
		List<IMyDoor> Copy=new List<IMyDoor>();
		string name=GetRemovedString(Door1.CustomName, "Door 1");
		foreach(IMyDoor Door2 in AllAirlockDoor2s){
			Copy.Add(Door2);
		}
		foreach(IMyDoor Door2 in GenericMethods<IMyDoor>.SortByDistance(Copy, Door1)){
			if(GetRemovedString(Door2.CustomName, "Door 2").Equals(name))
				pair.Add(Door2);
		}
		if(pair.Count > 1)
			PossibleAirlockDoor1Pairs.Add(pair);
	}
	List<List<IMyDoor>> PossibleAirlockDoor2Pairs=new List<List<IMyDoor>>();
	foreach(IMyDoor Door2 in AllAirlockDoor2s){
		List<IMyDoor> pair=new List<IMyDoor>();
		pair.Add(Door2);
		List<IMyDoor> Copy=new List<IMyDoor>();
		string name=GetRemovedString(Door2.CustomName, "Door 2");
		foreach(IMyDoor Door1 in AllAirlockDoor1s){
			Copy.Add(Door1);
		}
		foreach(IMyDoor Door1 in GenericMethods<IMyDoor>.SortByDistance(Copy, Door2)){
			if(GetRemovedString(Door1.CustomName, "Door 1").Equals(name))
				pair.Add(Door1);
		}
		if(pair.Count > 1){
			PossibleAirlockDoor2Pairs.Add(pair);
		}
	}
	int removed=0;
	do{
		removed=0;
		foreach(List<IMyDoor> pair1 in PossibleAirlockDoor1Pairs){
			if(pair1.Count<=1){
				IMyDoor Door=pair1[0];
				PossibleAirlockDoor1Pairs=RemoveDoor(PossibleAirlockDoor1Pairs, Door);
				PossibleAirlockDoor2Pairs=RemoveDoor(PossibleAirlockDoor2Pairs, Door);
				continue;
			}
			foreach(List<IMyDoor> pair2 in PossibleAirlockDoor2Pairs){
				if(pair2.Count<=1){
					IMyDoor Door=pair2[0];
					PossibleAirlockDoor1Pairs=RemoveDoor(PossibleAirlockDoor1Pairs, Door);
					PossibleAirlockDoor2Pairs=RemoveDoor(PossibleAirlockDoor2Pairs, Door);
					continue;
				}
				if(pair2[0].Equals(pair1[1]) && pair1[0].Equals(pair2[1])){
					removed++;
					IMyDoor Door1=pair1[0];
					IMyDoor Door2=pair2[0];
					Airlocks.Add(new Airlock(Door1, Door2));
					PossibleAirlockDoor1Pairs=RemoveDoor(RemoveDoor(PossibleAirlockDoor1Pairs, Door1), Door2);
					PossibleAirlockDoor2Pairs=RemoveDoor(RemoveDoor(PossibleAirlockDoor2Pairs, Door2), Door1);
					break;
				}
			}
		}
	} 
	while(removed > 0 && PossibleAirlockDoor1Pairs.Count > 0 && PossibleAirlockDoor2Pairs.Count > 0);
	foreach(List<IMyDoor> pair1 in PossibleAirlockDoor1Pairs){
		if(pair1.Count<=1){
			IMyDoor Door=pair1[0];
			PossibleAirlockDoor1Pairs=RemoveDoor(PossibleAirlockDoor1Pairs, Door);
			PossibleAirlockDoor2Pairs=RemoveDoor(PossibleAirlockDoor2Pairs, Door);
			continue;
		}
		IMyDoor Door1=pair1[0];
		IMyDoor Door2=pair1[1];
		Airlocks.Add(new Airlock(Door1, Door2));
		PossibleAirlockDoor1Pairs=RemoveDoor(RemoveDoor(PossibleAirlockDoor1Pairs, Door1), Door2);
		PossibleAirlockDoor2Pairs=RemoveDoor(RemoveDoor(PossibleAirlockDoor2Pairs, Door2), Door1);
	}
}

UpdateFrequency GetUpdateFrequency(){
	if(!Operational)
		return UpdateFrequency.None;
	if(Controller.IsUnderControl)
		return UpdateFrequency.Update1;
	if(Controller.GetShipVelocities().AngularVelocity.Length() > .1f)
		return UpdateFrequency.Update1;
	if((Controller.GetShipVelocities().LinearVelocity-RestingVelocity).Length() > .5)
		return UpdateFrequency.Update1;
	return UpdateFrequency.Update10;
}

string GetRemovedString(string big_string, string small_string){
	string output=big_string;
	if(big_string.Contains(small_string)){
		output=big_string.Substring(0, big_string.IndexOf(small_string))+big_string.Substring(big_string.IndexOf(small_string)+small_string.Length);
	}
	return output;
}

List<List<IMyDoor>> RemoveDoor(List<List<IMyDoor>> list, IMyDoor Door){
	List<List<IMyDoor>> output=new List<List<IMyDoor>>();
	Echo("\tRemoving Door \""+Door.CustomName+"\" from list["+list.Count+"]");
	if(list.Count == 0){
		return output;
	}
	string ExampleDoorName="";
	foreach(List<IMyDoor> sublist in list){
		if(sublist.Count > 0){
			ExampleDoorName=sublist[0].CustomName;
			break;
		}
	}
	
	bool is_leading_group=(ExampleDoorName.Contains("Door 1") && Door.CustomName.Contains("Door 1")) || (ExampleDoorName.Contains("Door 2") && Door.CustomName.Contains("Door 2"));
	for(int i=0; i<list.Count; i++){
		if(list[i].Count > 1 && (!is_leading_group || !list[i][0].Equals(Door))){
			if(is_leading_group){
				output.Add(list[i]);
			}
			else{
				List<IMyDoor> pair=new List<IMyDoor>();
				pair.Add(list[i][0]);
				for(int j=1; j<list[i].Count; j++){
					if(!list[i][j].Equals(Door))
						pair.Add(list[i][j]);
				}
				if(pair.Count>1)
					output.Add(pair);
			}
		}
	}
	return output;
}

public bool Setup(){
	Reset();
	
	StatusLCDs=(new GenericMethods<IMyTextPanel>(this)).GetAllConstruct("Ship Status");
	DebugLCDs=(new GenericMethods<IMyTextPanel>(this)).GetAllConstruct("AI Visual Display");
	foreach(IMyTextPanel Display in DebugLCDs){
		if(Display.CustomName.ToLower().Contains("transparent")){
			Display.FontColor=DEFAULT_BACKGROUND_COLOR;
			Display.BackgroundColor=new Color(0,0,0,0);
		}
		else{
			Display.FontColor=DEFAULT_TEXT_COLOR;
			Display.BackgroundColor=DEFAULT_BACKGROUND_COLOR;
		}
		Display.Alignment=TextAlignment.CENTER;
		Display.ContentType=ContentType.TEXT_AND_IMAGE;
		Display.TextPadding=10.0f;
		Display.FontSize=1.0f;
	}
	
	SetupAirlocks();
	AutoDoors=(new GenericMethods<IMyDoor>(this)).GetAllConstruct(AUTODOOR_NAME);
	
	Controller=(new GenericMethods<IMyShipController>(this)).GetClosestFunc(ControllerFunction);
	if(Controller==null){
		Write("Failed to find Controller", false, false);
		return false;
	}
	else {
		Forward=Controller.Orientation.Forward;
		Up=Controller.Orientation.Up;
		Left=Controller.Orientation.Left;
		Controller.CustomData="Forward:"+Forward.ToString();
		Controller.CustomData+="\nUp:"+Up.ToString();
		Controller.CustomData+="\nLeft:"+Left.ToString();
		IMyTextSurfaceProvider Cockpit=Controller as IMyTextSurfaceProvider;
		if(Cockpit!=null){
			for(int i=0;i<Cockpit.SurfaceCount;i++){
				Cockpit.GetSurface(i).FontColor=DEFAULT_TEXT_COLOR;
				Cockpit.GetSurface(i).BackgroundColor=DEFAULT_BACKGROUND_COLOR;
				Cockpit.GetSurface(i).Alignment=TextAlignment.CENTER;
				Cockpit.GetSurface(i).ScriptForegroundColor=DEFAULT_TEXT_COLOR;
				Cockpit.GetSurface(i).ScriptBackgroundColor=DEFAULT_BACKGROUND_COLOR;
			}
		}
		MySize=Controller.CubeGrid.GridSize;
	}
	Gyroscope=(new GenericMethods<IMyGyro>(this)).GetContaining("Control Gyroscope");
	if(Gyroscope==null){
		Write("Failed to find Gyroscope", false, false);
		if(!Me.CubeGrid.IsStatic)
			return false;
	}
	else {
		Gyroscope.CustomData="Forward:"+Gyroscope.Orientation.Forward.ToString();
		Gyroscope.CustomData+="\nUp:"+Gyroscope.Orientation.Up.ToString();
		Gyroscope.CustomData+="\nLeft:"+Gyroscope.Orientation.Left.ToString();
		Gyroscope.GyroOverride=Controller.IsUnderControl;
	}
	
	List<IMyThrust> MyThrusters=(new GenericMethods<IMyThrust>(this)).GetAllConstruct("");
	for(int i=0;i<2;i++){
		bool retry=!Me.CubeGrid.IsStatic;
		foreach(IMyThrust Thruster in MyThrusters){
			if(HasBlockData(Thruster, "Owner")){
				long ID=0;
				if(i==0 && !Int64.TryParse(GetBlockData(Thruster, "Owner"), out ID) || (ID != 0 && ID!=Me.CubeGrid.EntityId))
					continue;
			}
			if(Thruster.CubeGrid!=Controller.CubeGrid)
				continue;
			retry=false;
			Base6Directions.Direction ThrustDirection=Thruster.Orientation.Forward;
			if(ThrustDirection==Backward){
				Forward_Thrusters.Add(Thruster);
			}
			else if(ThrustDirection==Forward){
				Backward_Thrusters.Add(Thruster);
			}
			else if(ThrustDirection==Down){
				Up_Thrusters.Add(Thruster);
			}
			else if(ThrustDirection==Up){
				Down_Thrusters.Add(Thruster);
			}
			else if(ThrustDirection==Right){
				Left_Thrusters.Add(Thruster);
			}
			else if(ThrustDirection==Left){
				Right_Thrusters.Add(Thruster);
			}
		}
		if(!retry)
			break;
	}
	SetThrusterList(Forward_Thrusters, "Forward");
	SetThrusterList(Backward_Thrusters, "Backward");
	SetThrusterList(Up_Thrusters, "Up");
	SetThrusterList(Down_Thrusters, "Down");
	SetThrusterList(Left_Thrusters, "Left");
	SetThrusterList(Right_Thrusters, "Right");
	
	Operational=Me.IsWorking;
	Runtime.UpdateFrequency=GetUpdateFrequency();
	return true;
}

bool Operational=false;
public Program(){
	DebugLCDs=new List<IMyTextPanel>();
	Me.CustomName=(Program_Name+" Programmable block").Trim();
	for(int i=0;i<Me.SurfaceCount;i++){
		Me.GetSurface(i).FontColor=DEFAULT_TEXT_COLOR;
		Me.GetSurface(i).BackgroundColor=DEFAULT_BACKGROUND_COLOR;
		Me.GetSurface(i).Alignment=TextAlignment.CENTER;
		Me.GetSurface(i).ContentType=ContentType.TEXT_AND_IMAGE;
	}
	Me.GetSurface(1).FontSize=2.2f;
	Me.GetSurface(1).TextPadding=40.0f;
	Echo("Beginning initialization");
	Me.Enabled=true;
	Rnd=new Random();
	Setup();
	string[] args=this.Storage.Split('•');
	foreach(string arg in args){
		EntityInfo Entity=null;
		if(EntityInfo.TryParse(arg, out Entity)){
			switch(Entity.Type){
				case MyDetectedEntityType.Asteroid:
					AsteroidList.UpdateEntry(Entity);
					break;
				case MyDetectedEntityType.Planet:
					PlanetList.UpdateEntry(Entity);
					break;
				case MyDetectedEntityType.SmallGrid:
					SmallShipList.UpdateEntry(Entity);
					break;
				case MyDetectedEntityType.LargeGrid:
					LargeShipList.UpdateEntry(Entity);
					break;
				case MyDetectedEntityType.CharacterHuman:
					CharacterList.UpdateEntry(Entity);
					break;
				case MyDetectedEntityType.CharacterOther:
					CharacterList.UpdateEntry(Entity);
					break;
			}
		}
		else if(arg.IndexOf("Lockdown:")==0){
			bool.TryParse(arg.Substring("Lockdown:".Length), out _Lockdown);
		}
	}
	IGC.RegisterBroadcastListener("Neptine AI");
	IGC.RegisterBroadcastListener("Entity Report");
	IGC.RegisterBroadcastListener(Me.CubeGrid.CustomName);
	CreateMenu();
	DisplayMenu();
}

void SetThrusterList(List<IMyThrust> Thrusters, string Direction){
	int small_misc=0;
	int large_misc=0;
	int small_hydrogen=0;
	int large_hydrogen=0;
	int small_atmospheric=0;
	int large_atmospheric=0;
	int small_ion=0;
	int large_ion=0;
	
	foreach(IMyThrust Thruster in Thrusters){
		if(!HasBlockData(Thruster, "DefaultOverride")){
			SetBlockData(Thruster, "DefaultOverride", Thruster.ThrustOverridePercentage.ToString());
		}
		SetBlockData(Thruster, "Owner", Me.CubeGrid.EntityId.ToString());
		SetBlockData(Thruster, "DefaultName", Thruster.CustomName);
		if(Thruster.CustomName.ToLower().Contains("hydrogen")){
			if(Thruster.CustomName.ToLower().Contains("large"))
				large_hydrogen++;
			else
				small_hydrogen++;
		}
		else if(Thruster.CustomName.ToLower().Contains("atmospheric")){
			if(Thruster.CustomName.ToLower().Contains("large"))
				large_atmospheric++;
			else
				small_atmospheric++;
		}
		else if(Thruster.CustomName.ToLower().Contains("ion")){
			if(Thruster.CustomName.ToLower().Contains("large"))
				large_ion++;
			else
				small_ion++;
		}
		else{
			if(Thruster.CustomName.ToLower().Contains("large"))
				large_misc++;
			else
				small_misc++;
		}
	}
	foreach(IMyThrust Thruster in Thrusters){
		string tag;
		int number;
		if(Thruster.CustomName.ToLower().Contains("hydrogen")){
			if(Thruster.CustomName.ToLower().Contains("large")){
				tag="Large Hydrogen";
				number=large_hydrogen--;
			}
			else {
				tag="Small Hydrogen";
				number=small_hydrogen--;
			}
		}
		else if(Thruster.CustomName.ToLower().Contains("atmospheric")){
			if(Thruster.CustomName.ToLower().Contains("large")){
				tag="Large Atmospheric";
				number=large_atmospheric--;
			}
			else {
				tag="Small Atmospheric";
				number=small_atmospheric--;
			}
		}
		else if(Thruster.CustomName.ToLower().Contains("ion")){
			if(Thruster.CustomName.ToLower().Contains("large")){
				tag="Large Ion";
				number=large_ion--;
			}
			else {
				tag="Small Ion";
				number=small_ion--;
			}
		}
		else{
			if(Thruster.CustomName.ToLower().Contains("large")){
				tag="Large Misc";
				number=large_misc--;
			}
			else {
				tag="Small Misc";
				number=small_misc--;
			}
		}
		Thruster.CustomName=Direction+tag+" Thruster "+number.ToString();
	}
}

void ResetThruster(IMyThrust Thruster){
	if(HasBlockData(Thruster, "DefaultOverride")){
		float ThrustOverride=0.0f;
		if(float.TryParse(GetBlockData(Thruster, "DefaultOverride"), out ThrustOverride))
			Thruster.ThrustOverridePercentage=ThrustOverride;
		else
			Thruster.ThrustOverridePercentage=0.0f;
	}
	if(HasBlockData(Thruster, "DefaultName")){
		Thruster.CustomName=GetBlockData(Thruster, "DefaultName");
	}
	SetBlockData(Thruster, "Owner", "0");
}

public void Save(){
	this.Storage="Lockdown:"+_Lockdown.ToString();
	foreach(EntityInfo Entity in AsteroidList){
		this.Storage += '•'+Entity.ToString();
	}
	foreach(EntityInfo Entity in PlanetList){
		this.Storage += '•'+Entity.ToString();
	}
	foreach(EntityInfo Entity in SmallShipList){
		this.Storage += '•'+Entity.ToString();
	}
	foreach(EntityInfo Entity in LargeShipList){
		this.Storage += '•'+Entity.ToString();
	}
	foreach(EntityInfo Entity in CharacterList){
		this.Storage += '•'+Entity.ToString();
	}
	Me.CustomData=this.Storage;
	
	if(Gyroscope!=null)
		Gyroscope.GyroOverride=false;
	foreach(IMyThrust Thruster in Forward_Thrusters){
		ResetThruster(Thruster);
	}
	foreach(IMyThrust Thruster in Backward_Thrusters){
		ResetThruster(Thruster);
	}
	foreach(IMyThrust Thruster in Up_Thrusters){
		ResetThruster(Thruster);
	}
	foreach(IMyThrust Thruster in Down_Thrusters){
		ResetThruster(Thruster);
	}
	foreach(IMyThrust Thruster in Left_Thrusters){
		ResetThruster(Thruster);
	}
	foreach(IMyThrust Thruster in Right_Thrusters){
		ResetThruster(Thruster);
	}
}

public Vector3D GlobalToLocal(Vector3D Global){
	Vector3D Local=Vector3D.Transform(Global+Controller.GetPosition(), MatrixD.Invert(Controller.WorldMatrix));
	Local.Normalize();
	return Local*Global.Length();
}

public Vector3D GlobalToLocalPosition(Vector3D Global){
	Vector3D Local=Vector3D.Transform(Global, MatrixD.Invert(Controller.WorldMatrix));
	Local.Normalize();
	return Local*(Global-Controller.GetPosition()).Length();
}

public Vector3D LocalToGlobal(Vector3D Local){
	Vector3D Global=Vector3D.Transform(Local, Controller.WorldMatrix)-Controller.GetPosition();
	Global.Normalize();
	return Global*Local.Length();
}

public Vector3D LocalToGlobalPosition(Vector3D Local){
	return Vector3D.Transform(Local,Controller.WorldMatrix);
}

enum AlertStatus{
	Green=0,
	Blue=1,
	Yellow=2,
	Orange=3,
	Red=4
}
string Submessage="";
AlertStatus ShipStatus{
	get{
		AlertStatus status=AlertStatus.Green;
		Submessage="";
		if(!Me.CubeGrid.IsStatic){
			if(Elevation-MySize<50){
				AlertStatus new_status=AlertStatus.Blue;
				status=(AlertStatus) Math.Max((int)status, (int)new_status);
				double psuedo_elevation=Math.Max(Elevation-MySize,0);
				Submessage+="\nShip at low Altitude ("+Math.Round(psuedo_elevation,1).ToString()+"-"+Math.Round(Elevation,1).ToString()+" meters)";
			}
			
			if(Time_To_Crash>0){
				if(Time_To_Crash<15 && Controller.GetShipSpeed()>5){
					AlertStatus new_status=AlertStatus.Orange;
					status=(AlertStatus) Math.Max((int)status, (int)new_status);
					Submessage += "\n"+Math.Round(Time_To_Crash,1).ToString()+" seconds to possible impact";
				}
				else if(Time_To_Crash<60 && Controller.GetShipSpeed()>15){
					AlertStatus new_status=AlertStatus.Yellow;
					status=(AlertStatus) Math.Max((int)status, (int)new_status);
					Submessage += "\n"+Math.Round(Time_To_Crash,1).ToString()+" seconds to possible impact";
				}
				else if(Time_To_Crash<180){
					AlertStatus new_status=AlertStatus.Blue;
					status=(AlertStatus) Math.Max((int)status, (int)new_status);
					Submessage += "\n"+Math.Round(Time_To_Crash,1).ToString()+" seconds to possible impact";
				}
			}
			if(_Autoland&&CurrentVelocity.Length()>1){
				AlertStatus new_status=AlertStatus.Blue;
				status=(AlertStatus) Math.Max((int)status, (int)new_status);
				Submessage += "\nAutoland Enabled";
			}
		}
		if(_Lockdown){
			AlertStatus new_status=AlertStatus.Yellow;
			status=(AlertStatus) Math.Max((int)status, (int)new_status);
			Submessage += "\nCurrently in Lockdown";
		}
		
		double ActualEnemyShipDistance=Math.Min(SmallShipList.ClosestDistance(this, MyRelationsBetweenPlayerAndBlock.Enemies), LargeShipList.ClosestDistance(this, MyRelationsBetweenPlayerAndBlock.Enemies));
		double EnemyShipDistance=Math.Min(SmallShipList.ClosestDistance(this, MyRelationsBetweenPlayerAndBlock.Enemies), LargeShipList.ClosestDistance(this, MyRelationsBetweenPlayerAndBlock.Enemies)/2);
		if(EnemyShipDistance<800){
			AlertStatus new_status=AlertStatus.Red;
			status=(AlertStatus) Math.Max((int)status, (int)new_status);
			Submessage += "\nEnemy Ship at "+Math.Round(ActualEnemyShipDistance, 0)+" meters";
		}
		else if(EnemyShipDistance<2500){
			AlertStatus new_status=AlertStatus.Orange;
			status=(AlertStatus) Math.Max((int)status, (int)new_status);
			Submessage += "\nEnemy Ship at "+Math.Round(ActualEnemyShipDistance, 0)+" meters";
		}
		else if(EnemyShipDistance<5000){
			AlertStatus new_status=AlertStatus.Yellow;
			status=(AlertStatus) Math.Max((int)status, (int)new_status);
			Submessage += "\nEnemy Ship at "+Math.Round(ActualEnemyShipDistance, 0)+" meters";
		}
		
		double EnemyCharacterDistance=CharacterList.ClosestDistance(this, MyRelationsBetweenPlayerAndBlock.Enemies);
		if(EnemyCharacterDistance-MySize<0){
			AlertStatus new_status=AlertStatus.Red;
			status=(AlertStatus) Math.Max((int)status, (int)new_status);
			Submessage += "\nEnemy Creature has boarded ship";
		}
		else if(EnemyCharacterDistance-MySize<800){
			AlertStatus new_status=AlertStatus.Orange;
			status=(AlertStatus) Math.Max((int)status, (int)new_status);
			Submessage += "\nEnemy Creature at "+Math.Round(EnemyCharacterDistance, 0)+" meters";
		}
		else if(EnemyCharacterDistance-MySize<2000){
			AlertStatus new_status=AlertStatus.Yellow;
			status=(AlertStatus) Math.Max((int)status, (int)new_status);
			Submessage += "\nEnemy Creature at "+Math.Round(EnemyCharacterDistance, 0)+" meters";
		}
		
		double ShipDistance=Math.Min(SmallShipList.ClosestDistance(this),LargeShipList.ClosestDistance(this))-MySize;
		if(ShipDistance<500 && ShipDistance > 0){
			AlertStatus new_status=AlertStatus.Blue;
			status=(AlertStatus) Math.Max((int)status, (int)new_status);
			Submessage += "\nNearby ship at "+Math.Round(ShipDistance, 0)+" meters";
		}
		if((!Me.CubeGrid.IsStatic)&&AsteroidList.ClosestDistance(this)<500){
			AlertStatus new_status=AlertStatus.Blue;
			status=(AlertStatus) Math.Max((int)status, (int)new_status);
			Submessage += "\nNearby asteroid at "+Math.Round(AsteroidList.ClosestDistance(this), 0)+" meters";
		}
		if(Controller.GetShipSpeed() > 30){
			AlertStatus new_status=AlertStatus.Blue;
			status=(AlertStatus) Math.Max((int)status, (int)new_status);
			double Speed=Controller.GetShipSpeed();
			Submessage += "\nHigh Ship Speed [";
			const int SECTIONS=20;
			for(int i=0; i<SECTIONS; i++){
				if(Speed >= ((100.0/SECTIONS)*i)){
					Submessage += '|';
				}
				else {
					Submessage += ' ';
				}
			}
			Submessage += ']';
		}
		if(status == AlertStatus.Green)
			Submessage="\nNo issues";
		return status;
	}
}

void UpdateList(List<MyDetectedEntityInfo> list, MyDetectedEntityInfo new_entity){
	if(new_entity.Type == MyDetectedEntityType.None || new_entity.EntityId == Me.CubeGrid.EntityId)
		return;
	for(int i=0; i<list.Count; i++){
		if(list[i].EntityId == new_entity.EntityId){
			if(list[i].TimeStamp<new_entity.TimeStamp || ((list[i].HitPosition==null)&&(new_entity.HitPosition!=null)))
				list[i]=new_entity;
			return;
		}
	}
	list.Add(new_entity);
}

void UpdateList(List<EntityInfo> list, EntityInfo new_entity){
	if(new_entity.Type == MyDetectedEntityType.None || new_entity.ID == Me.CubeGrid.EntityId)
		return;
	for(int i=0; i<list.Count; i++){
		if(list[i].ID == new_entity.ID){
			list[i]=new_entity;
			return;
		}
	}
	list.Add(new_entity);
}

void SetStatus(string message, Color TextColor, Color BackgroundColor){
	float padding=40.0f;
	string[] lines=message.Split('\n');
	padding=Math.Max(10.0f, padding-(lines.Length*5.0f));
	foreach(IMyTextPanel LCD in StatusLCDs){
		LCD.Alignment=TextAlignment.CENTER;
		LCD.FontSize=1.2f;
		LCD.ContentType=ContentType.TEXT_AND_IMAGE;
		LCD.TextPadding=padding;
		LCD.WriteText(message, false);
		if(LCD.CustomName.ToLower().Contains("transparent")){
			LCD.FontColor=BackgroundColor;
			LCD.BackgroundColor=new Color(0,0,0,255);
		}
		else {
			LCD.FontColor=TextColor;
			LCD.BackgroundColor=BackgroundColor;
		}
	}
}

public bool Stop(object obj=null){
	RestingVelocity=new Vector3D(0,0,0);
	Target_Position=new Vector3D(0,0,0);
	Match_Direction=false;
	Match_Position=false;
	Tracking=false;
	Target_ID=0;
	return true;
}

bool _Autoland=false;
public bool Autoland(object obj=null){
	_Autoland=!_Autoland;
	return true;
}

bool _Lockdown=false;
public bool Lockdown(object obj=null){
	_Lockdown=!_Lockdown;
	List<IMyDoor> Seals=(new GenericMethods<IMyDoor>(this)).GetAllIncluding("Air Seal");
	foreach(IMyDoor Door in Seals){
		if(CanHaveJob(Door, "Lockdown")){
			if(_Lockdown){
				SetBlockData(Door, "Job", "Lockdown");
				Door.Enabled=true;
				Door.CloseDoor();
			}
			else{
				SetBlockData(Door, "Job", "None");
				Door.Enabled=true;
				Door.OpenDoor();
			}
		}
	}
	if(LOCKDOWN_LIGHT_NAME.Length>0){
		List<IMyInteriorLight> Lights=(new GenericMethods<IMyInteriorLight>(this)).GetAllIncluding(LOCKDOWN_LIGHT_NAME);
		foreach(IMyInteriorLight Light in Lights){
			if(CanHaveJob(Light, "Lockdown")){
				if(_Lockdown){
					SetBlockData(Light,"Job","Lockdown");
					if(!HasBlockData(Light,"DefaultColor"))
						SetBlockData(Light,"DefaultColor",Light.Color.ToString());
					Light.Color=LOCKDOWN_COLOR;
				}
				else{
					SetBlockData(Light,"Job","None");
					if(HasBlockData(Light, "DefaultColor")){
						try{
							Light.Color=ColorParse(GetBlockData(Light,"DefaultColor"));
						}
						catch(Exception){
							Echo("Failed to parse color");
						}
					}
				}
			}
		}
	}
	return true;
}

public bool FactoryReset(object obj=null){
	SetStatus("Status LCD\nOffline", DEFAULT_TEXT_COLOR, DEFAULT_BACKGROUND_COLOR);
	if(Gyroscope!=null)
		Gyroscope.GyroOverride=false;
	foreach(IMyThrust Thruster in Forward_Thrusters){
		ResetThruster(Thruster);
	}
	foreach(IMyThrust Thruster in Backward_Thrusters){
		ResetThruster(Thruster);
	}
	foreach(IMyThrust Thruster in Up_Thrusters){
		ResetThruster(Thruster);
	}
	foreach(IMyThrust Thruster in Down_Thrusters){
		ResetThruster(Thruster);
	}
	foreach(IMyThrust Thruster in Left_Thrusters){
		ResetThruster(Thruster);
	}
	foreach(IMyThrust Thruster in Right_Thrusters){
		ResetThruster(Thruster);
	}
	AsteroidList.Clear();
	PlanetList.Clear();
	SmallShipList.Clear();
	LargeShipList.Clear();
	CharacterList.Clear();
	Me.CustomData="";
	this.Storage="";
	
	Reset();
	Me.Enabled=false;
	return true;
}

Vector3D GetOffsetPosition(Vector3D Position, double Target_Size=0){
	Vector3D direction=Position-Controller.GetPosition();
	double distance=direction.Length();
	direction.Normalize();
	if(distance>1000){
		distance-=400;
	}
	else{
		distance=distance/10*9;
	}
	distance-=Target_Size;
	double controller_offset=(Controller.GetPosition()-Controller.GetPosition()).Length();
	distance-=Math.Max(0,MySize/2-controller_offset);
	return (distance*direction)+Controller.GetPosition();
}

public bool GoTo(EntityInfo Entity){
	//Match velocity for hostiles, GoTo for others
	if(Target_ID==Entity.ID){
		return Stop();
	}
	Controller.DampenersOverride=true;
	RestingVelocity=Entity.Velocity;
	Target_ID=Entity.ID;
	Tracking=true;
	if(Entity.Relationship!=MyRelationsBetweenPlayerAndBlock.Enemies){
		Target_Direction=Entity.Position-Controller.GetPosition();
		Target_Direction.Normalize();
		Target_Position=GetOffsetPosition(Entity.Position, Entity.Size);
		Match_Position=true;
		Match_Direction=true;
	}
	else{
		Target_Position=Entity.Position;
	}
	return true;
}

public bool DeployFlares(IMyProgrammableBlock FlareBlock){
	return FlareBlock.TryRun("Deploy");
}

public bool UpdateEntityListing(Menu_Submenu Menu){
	EntityList list=null;
	bool do_goto=false;
	switch(Menu.Name()){
		case "Asteroids":
			list=AsteroidList;
			do_goto=true;
			break;
		case "Planets":
			list=PlanetList;
			do_goto=false;
			break;
		case "Small Ships":
			list=SmallShipList;
			do_goto=true;
			break;
		case "Large Ships":
			list=LargeShipList;
			do_goto=true;
			break;
		case "Characters":
			list=CharacterList;
			do_goto=true;
			break;
	}
	if(list==null)
		return false;
	Menu=new Menu_Submenu(Menu.Name());
	Menu.Add(new Menu_Command<Menu_Submenu>("Refresh", UpdateEntityListing, "Updates "+Menu.Name(), Menu));
	list.Sort(Controller.GetPosition());
	for(int i=0;i<list.Count;i++){
		if(do_goto&&!Me.CubeGrid.IsStatic)
			Menu.Add(new Menu_Display(list[i], this, GoTo));
		else
			Menu.Add(new Menu_Display(list[i], this));
	}
	if(Command_Menu.Replace(Menu)){
		DisplayMenu();
		return true;
	}
	return false;
}

Menu_Submenu AsteroidMenu;
Menu_Submenu PlanetMenu;
Menu_Submenu SmallShipMenu;
Menu_Submenu LargeShipMenu;
Menu_Submenu CharacterMenu;
bool CreateMenu(object obj=null){
	Command_Menu=new Menu_Submenu("Command Menu");
	Command_Menu.Add(new Menu_Command<object>("Update Menu", CreateMenu, "Refreshes menu"));
	Menu_Submenu ShipCommands=new Menu_Submenu("Commands");
	if(!Me.CubeGrid.IsStatic){
		ShipCommands.Add(new Menu_Command<object>("Stop", Stop, "Disables autopilot"));
		ShipCommands.Add(new Menu_Command<object>("Toggle Autoland",Autoland,"Toggles On/Off the Autoland feature\nLands at 5 m/s\nDo not use on ships with poor mobility!"));
	}
	ShipCommands.Add(new Menu_Command<object>("Scan", PerformScan, "Immediately performs a scan operation"));
	IMyProgrammableBlock FlareBlock=(new GenericMethods<IMyProgrammableBlock>(this)).GetFull("Flare Printer Programmable block");
	if(FlareBlock!=null)
		ShipCommands.Add(new Menu_Command<IMyProgrammableBlock>("Deploy Flares",DeployFlares,"Deploys flares made using Flare Printers",FlareBlock));
	ShipCommands.Add(new Menu_Command<object>("Toggle Lockdown", Lockdown, "Closes/Opens Air Seals"));
	ShipCommands.Add(new Menu_Command<object>("Factory Reset", FactoryReset, "Resets AI memory and settings, and turns it off"));
	Command_Menu.Add(ShipCommands);
	AsteroidMenu=new Menu_Submenu("Asteroids");
	Command_Menu.Add(AsteroidMenu);
	UpdateEntityListing(AsteroidMenu);
	PlanetMenu=new Menu_Submenu("Planets");
	Command_Menu.Add(PlanetMenu);
	UpdateEntityListing(PlanetMenu);
	SmallShipMenu=new Menu_Submenu("Small Ships");
	Command_Menu.Add(SmallShipMenu);
	UpdateEntityListing(SmallShipMenu);
	LargeShipMenu=new Menu_Submenu("Large Ships");
	Command_Menu.Add(LargeShipMenu);
	UpdateEntityListing(LargeShipMenu);
	CharacterMenu=new Menu_Submenu("Characters");
	Command_Menu.Add(CharacterMenu);
	UpdateEntityListing(CharacterMenu);
	return true;
}

void DisplayMenu(){
	List<IMyTextPanel> Panels=(new GenericMethods<IMyTextPanel>(this)).GetAllConstruct("Command Menu Display");
	foreach(IMyTextPanel Panel in Panels){
		Panel.WriteText(Command_Menu.ToString(), false);
		Panel.Alignment=TextAlignment.CENTER;
		Panel.FontSize=1.2f;
		Panel.ContentType=ContentType.TEXT_AND_IMAGE;
		Panel.TextPadding=10.0f;
		if(Panel.CustomName.ToLower().Contains("transparent")){
			Panel.FontColor=DEFAULT_BACKGROUND_COLOR;
			Panel.BackgroundColor=new Color(0,0,0,0);
		}
		else{
			Panel.FontColor=DEFAULT_TEXT_COLOR;
			Panel.BackgroundColor=DEFAULT_BACKGROUND_COLOR;
		}
	}
}

Color ColorParse(string parse){
	parse=parse.Substring(parse.IndexOf('{')+1);
	parse=parse.Substring(0, parse.IndexOf('}')-1);
	string[] args=parse.Split(' ');
	int r, g, b, a;
	r=Int32.Parse(args[0].Substring(args[0].IndexOf("R:")+2).Trim());
	g=Int32.Parse(args[1].Substring(args[1].IndexOf("G:")+2).Trim());
	b=Int32.Parse(args[2].Substring(args[2].IndexOf("B:")+2).Trim());
	a=Int32.Parse(args[3].Substring(args[3].IndexOf("A:")+2).Trim());
	return new Color(r,g,b,a);
}

bool last_performed_alarm=false;
public void PerformAlarm(){
	bool nearby_enemy=(CharacterList.ClosestDistance(this, MyRelationsBetweenPlayerAndBlock.Enemies) <= (float) MySize);
	if(!nearby_enemy && !last_performed_alarm){
		return;
	}
	last_performed_alarm=nearby_enemy;
	List<IMyInteriorLight> AllLights=new List<IMyInteriorLight>();
	GridTerminalSystem.GetBlocksOfType<IMyInteriorLight>(AllLights);
	foreach(IMyInteriorLight Light in AllLights){
		if(!CanHaveJob(Light, "PlayerAlert"))
			continue;
		double distance=double.MaxValue;
		foreach(EntityInfo Entity in CharacterList){
			if((Entity.Relationship == MyRelationsBetweenPlayerAndBlock.Enemies) && Entity.Age.TotalSeconds <= 60)
				distance=Math.Min(distance, Entity.GetDistance(Light.GetPosition()));
		}
		if(distance <= ALERT_DISTANCE){
			if(!HasBlockData(Light, "DefaultColor")){
				SetBlockData(Light, "DefaultColor", Light.Color.ToString());
			}
			if(!HasBlockData(Light, "DefaultBlinkLength")){
				SetBlockData(Light, "DefaultBlinkLength", Light.BlinkLength.ToString());
			}
			if(!HasBlockData(Light, "DefaultBlinkInterval")){
				SetBlockData(Light, "DefaultBlinkInterval", Light.BlinkIntervalSeconds.ToString());
			}
			SetBlockData(Light, "Job", "PlayerAlert");
			Light.Color=new Color(255, 0, 0, 255);
			Light.BlinkLength=100.0f-(((float) (distance / ALERT_DISTANCE))*50.0f);
			Light.BlinkIntervalSeconds=1.0f;
		}
		else {
			if(HasBlockData(Light, "Job") && GetBlockData(Light, "Job").Equals("PlayerAlert")){
				if(HasBlockData(Light, "DefaultColor")){
					try{
						Light.Color=ColorParse(GetBlockData(Light, "DefaultColor"));
					}
					catch(Exception){
						Echo("Failed to parse color");
					}
				}
				if(HasBlockData(Light, "DefaultBlinkLength")){
					try{
						Light.BlinkLength=float.Parse(GetBlockData(Light, "DefaultBlinkLength"));
					}
					catch(Exception){
						;
					}
				}
				if(HasBlockData(Light, "DefaultBlinkInterval")){
					try{
						Light.BlinkIntervalSeconds=float.Parse(GetBlockData(Light, "DefaultBlinkInterval"));
					}
					catch(Exception){
						;
					}
				}
				SetBlockData(Light, "Job", "None");
			}
		}
	}
	List<IMySoundBlock> AllSounds=new List<IMySoundBlock>();
	GridTerminalSystem.GetBlocksOfType<IMySoundBlock>(AllSounds);
	foreach(IMySoundBlock Sound in AllSounds){
		if(!CanHaveJob(Sound, "PlayerAlert"))
			continue;
		double distance=double.MaxValue;
		foreach(EntityInfo Entity in CharacterList){
			if((Entity.Relationship == MyRelationsBetweenPlayerAndBlock.Enemies) && Entity.Age.TotalSeconds <= 60)
				distance=Math.Min(distance, Entity.GetDistance(Sound.GetPosition()));
		}
		if(distance <= ALERT_DISTANCE){
			if(!HasBlockData(Sound, "DefaultSound")){
				SetBlockData(Sound, "DefaultSound", Sound.SelectedSound);
			}
			if(!HasBlockData(Sound, "DefaultVolume")){
				SetBlockData(Sound, "DefaultVolume", Sound.Volume.ToString());
			}
			SetBlockData(Sound, "Job", "PlayerAlert");
			if(!HasBlockData(Sound, "Playing") || !GetBlockData(Sound, "Playing").Equals("True")){
				Sound.SelectedSound="SoundBlockEnemyDetected";
				Sound.Volume=100.0f;
				Sound.Play();
				SetBlockData(Sound, "Playing", "True");
			}
		}
		else {
			if(HasBlockData(Sound, "Job") && GetBlockData(Sound, "Job").Equals("PlayerAlert")){
				if(HasBlockData(Sound, "DefaultSound")){
					Sound.SelectedSound=GetBlockData(Sound, "DefaultSound");
				}
				if(HasBlockData(Sound, "DefaultVolume")){
					try{
						Sound.Volume=float.Parse(GetBlockData(Sound, "DefaultVolume"));
					}
					catch(Exception){
						;
					}
				}
				Sound.Stop();
				SetBlockData(Sound, "Playing", "False");
				SetBlockData(Sound, "Job", "None");
			}
		}
	}
	List<IMyDoor> AllDoors=new List<IMyDoor>();
	GridTerminalSystem.GetBlocksOfType<IMyDoor>(AllDoors);
	foreach(IMyDoor Door in AllDoors){
		if(!CanHaveJob(Door, "PlayerAlert"))
			continue;
		double distance=double.MaxValue;
		double friendly=double.MaxValue;
		foreach(EntityInfo Entity in CharacterList){
			if(Entity.Age.TotalSeconds<=60){
			if(Entity.Relationship == MyRelationsBetweenPlayerAndBlock.Enemies)
				distance=Math.Min(distance, Entity.GetDistance(Door.GetPosition()));
			else if(Entity.Relationship != MyRelationsBetweenPlayerAndBlock.Neutral)
				friendly=Math.Min(friendly, Entity.GetDistance(Door.GetPosition()));
			}
		}
		if(distance <= ALERT_DISTANCE && distance < friendly){
			if(!HasBlockData(Door, "DefaultState")){
				if(Door.Status.ToString().Contains("Open")){
					SetBlockData(Door, "DefaultState", "Open");
				}
				else {
					SetBlockData(Door, "DefaultState", "Closed");
				}
			}
			if(!HasBlockData(Door, "DefaultPower")){
				if(Door.Enabled){
					SetBlockData(Door, "DefaultPower", "On");
				}
				else {
					SetBlockData(Door, "DefaultPower", "Off");
				}
			}
			SetBlockData(Door, "Job", "PlayerAlert");
			Door.Enabled=(Door.Status != DoorStatus.Closed);
			Door.CloseDoor();
		}
		else {
			if(HasBlockData(Door, "Job") && GetBlockData(Door, "Job").Equals("PlayerAlert")){
				if(HasBlockData(Door, "DefaultPower")){
					Door.Enabled=GetBlockData(Door, "DefaultPower").Equals("On");
				}
				if(HasBlockData(Door, "DefaultState")){
					if(GetBlockData(Door, "DefaultState").Equals("Open"))
						Door.OpenDoor();
					else
						Door.CloseDoor();
				}
				SetBlockData(Door, "Job", "None");
			}
		}
	}
}

void UpdateAirlock(Airlock airlock){
	if(airlock.Door1.Status != DoorStatus.Closed && airlock.Door2.Status != DoorStatus.Closed){
		airlock.Door1.Enabled=true;
		airlock.Door1.CloseDoor();
		airlock.Door2.Enabled=true;
		airlock.Door2.CloseDoor();
	}
	if(!(CanHaveJob(airlock.Door1, "Airlock")&&(CanHaveJob(airlock.Door2, "Airlock"))))
		return;
	bool detected=false;
	double min_distance_1=double.MaxValue;
	double min_distance_2=double.MaxValue;
	double min_distance_check=3.75*(1+(Controller.GetShipSpeed() / 200));
	foreach(EntityInfo Entity in CharacterList){
		if(Entity.Relationship != MyRelationsBetweenPlayerAndBlock.Enemies && Entity.Relationship != MyRelationsBetweenPlayerAndBlock.Neutral){
			Vector3D position=Entity.Position+CurrentVelocity / 100;
			double distance=airlock.Distance(Entity.Position);
			bool is_closest_to_this_airlock=distance <= min_distance_check;
			if(is_closest_to_this_airlock){
				foreach(Airlock alock in Airlocks){
					if(is_closest_to_this_airlock && !alock.Equals(airlock)){
						is_closest_to_this_airlock=is_closest_to_this_airlock && distance<(alock.Distance(position));
					}
				}
			}
			if(is_closest_to_this_airlock){
				detected=true;
				min_distance_1=Math.Min(min_distance_1, (airlock.Door1.GetPosition()-position).Length());
				min_distance_2=Math.Min(min_distance_2, (airlock.Door2.GetPosition()-position).Length());
			}
		}
	}
	
	if(detected){
		SetBlockData(airlock.Door1, "Job", "Airlock");
		SetBlockData(airlock.Door2, "Job", "Airlock");
		if(min_distance_1 <= min_distance_2){
			airlock.Door2.Enabled=(airlock.Door2.Status != DoorStatus.Closed);
			if(airlock.Door2.Status != DoorStatus.Closing)
				airlock.Door2.CloseDoor();
			if(airlock.Door2.Enabled){
				airlock.Door1.Enabled=(airlock.Door1.Status != DoorStatus.Closed);
				if(airlock.Door1.Status != DoorStatus.Closing)
					airlock.Door1.CloseDoor();
				AirlockString += '\t'+"Closing Door 2"+'\n';
			}
			else {
				airlock.Door1.Enabled=true;
				if(airlock.Door1.Status != DoorStatus.Opening)
					airlock.Door1.OpenDoor();
				AirlockString += '\t'+"Opening Door 1"+'\n';
			}
		}
		else {
			airlock.Door1.Enabled=(airlock.Door1.Status != DoorStatus.Closed);
			if(airlock.Door1.Status != DoorStatus.Closing)
				airlock.Door1.CloseDoor();
			if(airlock.Door1.Enabled){
				airlock.Door2.Enabled=(airlock.Door2.Status != DoorStatus.Closed);
				if(airlock.Door2.Status != DoorStatus.Closing)
					airlock.Door2.CloseDoor();
				AirlockString += '\t'+"Closing Door 1"+'\n';
			}
			else {
				airlock.Door2.Enabled=true;
				if(airlock.Door2.Status != DoorStatus.Opening)
					airlock.Door2.OpenDoor();
				AirlockString += '\t'+"Opening Door 2"+'\n';
			}
		}
	}
	else{
		SetBlockData(airlock.Door1, "Job", "None");
		SetBlockData(airlock.Door2, "Job", "None");
		airlock.Door1.Enabled=(airlock.Door1.Status != DoorStatus.Closed);
		if(airlock.Door1.Status != DoorStatus.Closing)
			airlock.Door1.CloseDoor();
		airlock.Door2.Enabled=(airlock.Door2.Status != DoorStatus.Closed);
		if(airlock.Door2.Status != DoorStatus.Closing)
			airlock.Door2.CloseDoor();
		AirlockString += '\t'+"Closing both Doors"+'\n';
	}
}

List<IMyCameraBlock> GetValidCameras(){
	List<IMyCameraBlock> AllCameras=new List<IMyCameraBlock>();
	List<IMyCameraBlock> output=new List<IMyCameraBlock>();
	GridTerminalSystem.GetBlocksOfType<IMyCameraBlock>(AllCameras);
	foreach(IMyCameraBlock Camera in AllCameras){
		if(!HasBlockData(Camera, "DoRaycast")){
			SetBlockData(Camera, "DoRaycast", "maybe");
			SetBlockData(Camera, "RaycastTestCount", "0");
		}
		if(GetBlockData(Camera, "DoRaycast").Equals("false")){
			Camera.EnableRaycast=false;
			continue;
		}
		Camera.EnableRaycast=true;
		output.Add(Camera);
	}
	return output;
}

//Performs the scan function on all scanning devices
Vector3D Closest_Hit_Position=new Vector3D(0,0,0);
double Scan_Frequency{
	get{
		double output=10;
		double MySize=Me.CubeGrid.GridSize;
		output=Math.Max(1,Math.Min(output, 11-(CurrentVelocity.Length()/10)));
		double distance=SmallShipList.ClosestDistance(this);
		if(distance >= MySize){
			output=Math.Min(output,Math.Max(1, Math.Min(10,(distance+MySize+100)/100)));
		}
		distance=SmallShipList.ClosestDistance(this, MyRelationsBetweenPlayerAndBlock.Enemies);
		output=Math.Min(output,Math.Max(1, Math.Min(10,(distance-MySize+100)/100)));
		distance=LargeShipList.ClosestDistance(this);
		if(distance >= MySize){
			output=Math.Min(output,Math.Max(1, Math.Min(10,(distance+MySize+100)/100)));
		}
		distance=LargeShipList.ClosestDistance(this, MyRelationsBetweenPlayerAndBlock.Enemies);
		output=Math.Min(output,Math.Max(1, Math.Min(10,(distance-MySize+100)/100)));
		distance=CharacterList.ClosestDistance(this);
		output=Math.Min(output,Math.Max(1, Math.Min(10,(distance+MySize+100)/100)));
		if(Controller.IsUnderControl)
			output=Math.Min(output,5);
		return output;
	}
}

bool CanDetect(IMySensorBlock Sensor,MyRelationsBetweenPlayerAndBlock Relationship){
	switch(Relationship){
		case MyRelationsBetweenPlayerAndBlock.Owner:
			return Sensor.DetectOwner;
		case MyRelationsBetweenPlayerAndBlock.Friends:
			return Sensor.DetectFriendly;
		case MyRelationsBetweenPlayerAndBlock.Enemies:
			return Sensor.DetectEnemy;
	}
	return Sensor.DetectNeutral;
}

bool CanDetect(IMySensorBlock Sensor,MyDetectedEntityType Type){
	switch(Type){
		case MyDetectedEntityType.SmallGrid:
			return Sensor.DetectSmallShips;
		case MyDetectedEntityType.LargeGrid:
			return Sensor.DetectLargeShips&&Sensor.DetectStations;
		case MyDetectedEntityType.CharacterHuman:
			return Sensor.DetectPlayers;
		case MyDetectedEntityType.CharacterOther:
			return Sensor.DetectPlayers;
		case MyDetectedEntityType.Asteroid:
			return Sensor.DetectAsteroids;
		case MyDetectedEntityType.Planet:
			return Sensor.DetectAsteroids;
	}
	return false;
}

bool CanDetect(IMySensorBlock Sensor,EntityInfo Entity){
	return Sensor.IsWorking&&CanDetect(Sensor,Entity.Type)&&CanDetect(Sensor,Entity.Relationship);
}

bool IsDetecting(IMySensorBlock Sensor,EntityInfo Entity){
	List<MyDetectedEntityInfo> Entities=new List<MyDetectedEntityInfo>();
	Sensor.DetectedEntities(Entities);
	foreach(MyDetectedEntityInfo entity in Entities){
		if(entity.EntityId==Entity.ID || (Entity.GetDistance(entity.Position)<=0.5f&&Entity.Type==entity.Type))
			return true;
	}
	return false;
}

float SensorRange(IMySensorBlock Sensor){
	float range=50;
	range=Math.Min(range,Sensor.LeftExtend);
	range=Math.Min(range,Sensor.RightExtend);
	range=Math.Min(range,Sensor.TopExtend);
	range=Math.Min(range,Sensor.BottomExtend);
	range=Math.Min(range,Sensor.FrontExtend);
	range=Math.Min(range,Sensor.BackExtend);
	return range;
}

double Scan_Time=10;
string ScanString="";
string AirlockString="";
public double MySize=0;
public bool PerformScan(object obj=null){
	Write("Beginning Scan");
	ScanString="";
	AirlockString="";
	AsteroidList.UpdatePositions(Scan_Time);
	PlanetList.UpdatePositions(Scan_Time);
	SmallShipList.UpdatePositions(Scan_Time);
	LargeShipList.UpdatePositions(Scan_Time);
	CharacterList.UpdatePositions(Scan_Time);
	PerformDisarm();
	List<IMyTerminalBlock> AllTerminalBlocks=new List<IMyTerminalBlock>();
	GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(AllTerminalBlocks);
	MySize=0;
	foreach(IMyTerminalBlock Block in AllTerminalBlocks){
		double distance=(Controller.GetPosition()-Block.GetPosition()).Length();
		MySize=Math.Max(MySize,distance);
	}
	
	List<MyDetectedEntityInfo> DetectedEntities=new List<MyDetectedEntityInfo>();
	List<IMySensorBlock> AllSensors=new List<IMySensorBlock>();
	GridTerminalSystem.GetBlocksOfType<IMySensorBlock>(AllSensors);
	foreach(IMySensorBlock Sensor in AllSensors){
		UpdateList(DetectedEntities, Sensor.LastDetectedEntity);
		List<MyDetectedEntityInfo> entities=new List<MyDetectedEntityInfo>();
		Sensor.DetectedEntities(entities);
		foreach(MyDetectedEntityInfo Entity in entities){
			UpdateList(DetectedEntities, Entity);
		}
	}
	List<IMyLargeTurretBase> AllTurrets=new List<IMyLargeTurretBase>();
	GridTerminalSystem.GetBlocksOfType<IMyLargeTurretBase>(AllTurrets);
	foreach(IMyLargeTurretBase Turret in AllTurrets){
		if(Turret.HasTarget){
			UpdateList(DetectedEntities, Turret.GetTargetedEntity());
		}
	}
	
	List<bool> raycast_check=new List<bool>();
	for(int i=0;i<DetectedEntities.Count;i++)
		raycast_check.Add(false);
	
	double Scan_Crash_Time=double.MaxValue;
	List<IMyCameraBlock> MyCameras=GetValidCameras();
	foreach(IMyCameraBlock Camera in MyCameras){
		int count=0;
		bool update_me=false;
		if(GetBlockData(Camera, "DoRaycast").Equals("maybe")){
			Int32.TryParse(GetBlockData(Camera, "RaycastTestCount"), out count);
			if(count>=100){
				SetBlockData(Camera, "DoRaycast", "false");
				continue;
			}
			update_me=true;
		}
		double raycast_distance=RAYCAST_DISTANCE;
		if(Camera.RaycastDistanceLimit != -1){
			raycast_distance=Math.Min(raycast_distance, Camera.AvailableScanRange);
		}
		MyDetectedEntityInfo Raycast_Entity=Camera.Raycast(raycast_distance, 0, 0);
		if(update_me && Raycast_Entity.EntityId != Me.CubeGrid.EntityId && Raycast_Entity.EntityId != Camera.CubeGrid.EntityId){
			SetBlockData(Camera, "DoRaycast", "true");
			update_me=true;
		}
		UpdateList(DetectedEntities,Raycast_Entity);
		
		if(!update_me){
			if((Time_To_Crash<60 && Elevation<500)||Target_Distance<250){
				Vector3D Velocity_Direction=CurrentVelocity;
				Velocity_Direction.Normalize();
				double speed_check=15*CurrentVelocity.Length();
				double distance=Math.Min(speed_check, Camera.AvailableScanRange/2);
				if(Camera.CanScan(distance,Velocity_Direction)){
					MyDetectedEntityInfo Entity=Camera.Raycast(distance,Velocity_Direction);
					if(Entity.Type!=MyDetectedEntityType.None&&Entity.EntityId!=Me.CubeGrid.EntityId){
						try{
							double new_distance=(((Vector3D)Entity.HitPosition)-Camera.GetPosition()).Length();
							double new_time=new_distance/CurrentVelocity.Length();
							if(new_time<Scan_Crash_Time){
								Scan_Crash_Time=new_time;
								Closest_Hit_Position=((Vector3D)Entity.HitPosition);
							}
						}
						catch(Exception){
							;
						}
					}
				}
			}
			if(Tracking && RestingVelocity.Length()>0 && Target_Distance<=RAYCAST_DISTANCE){
				if(Camera.CanScan(Target_Position)){
					MyDetectedEntityInfo Entity=Camera.Raycast(Target_Position);
					if(Entity.Type!=MyDetectedEntityType.None&&Entity.EntityId!=Me.CubeGrid.EntityId){
						UpdateList(DetectedEntities,Entity);
						RestingVelocity=Entity.Velocity;
					}
				}
			}
			for(int i=0; i<Math.Min(raycast_check.Count,DetectedEntities.Count); i++){
				if(raycast_check[i] && Camera.CanScan(DetectedEntities[i].Position)){
					Raycast_Entity=Camera.Raycast(DetectedEntities[i].Position);
					UpdateList(DetectedEntities, Raycast_Entity);
					if(Raycast_Entity.Type!=MyDetectedEntityType.None&&Raycast_Entity.EntityId!=Me.CubeGrid.EntityId){
						raycast_check[i]=false;
					}
				}
			}
		}
	}
	
	ScanString += "Retrieved updated data\non "+DetectedEntities.Count+" relevant entities"+'\n';
	List<IMyBroadcastListener> listeners=new List<IMyBroadcastListener>();
	IGC.GetBroadcastListeners(listeners);
	MyDetectedEntityType MyType=MyDetectedEntityType.LargeGrid;
	if(Controller.CubeGrid.GridSizeEnum==MyCubeSize.Small)
		MyType=MyDetectedEntityType.SmallGrid;
	EntityInfo Myself = new EntityInfo(Controller.CubeGrid.EntityId,Controller.CubeGrid.CustomName,MyType,(Vector3D?)(Controller.GetPosition()),CurrentVelocity,MyRelationsBetweenPlayerAndBlock.Owner,Controller.CubeGrid.GetPosition());
	
	foreach(IMyBroadcastListener Listener in listeners){
		IGC.SendBroadcastMessage(Listener.Tag, Myself.ToString(), TransmissionDistance.TransmissionDistanceMax);
	}
	foreach(MyDetectedEntityInfo entity in DetectedEntities){
		EntityInfo Entity=new EntityInfo(entity);
		foreach(IMyBroadcastListener Listener in listeners){
			IGC.SendBroadcastMessage(Listener.Tag, Entity.ToString(), TransmissionDistance.TransmissionDistanceMax);
		}
	}
	
	List<EntityInfo> Entities=new List<EntityInfo>();
	foreach(MyDetectedEntityInfo entity in DetectedEntities){
		Entities.Add(new EntityInfo(entity));
	}
	
	foreach(IMyBroadcastListener Listener in listeners){
		int count=0;
		while(Listener.HasPendingMessage){
			MyIGCMessage message=Listener.AcceptMessage();
			count++;
			EntityInfo Entity;
			if(EntityInfo.TryParse(message.Data.ToString(), out Entity))
				UpdateList(Entities, Entity);
		}
		if(count>0)
			ScanString += "Received "+count.ToString()+" messages on "+Listener.Tag+'\n';
	}
	
	foreach(EntityInfo Entity in Entities){
		if(Entity.ID==Target_ID){
			RestingVelocity=Entity.Velocity;
			Target_Direction=Entity.Position-Controller.GetPosition();
			Target_Direction.Normalize();
			Target_Position=GetOffsetPosition(Entity.Position, Entity.Size);
		}
		switch(Entity.Type){
			case MyDetectedEntityType.Asteroid:
				AsteroidList.UpdateEntry(Entity);
				break;
			case MyDetectedEntityType.Planet:
				PlanetList.UpdateEntry(Entity);
				break;
			case MyDetectedEntityType.SmallGrid:
				SmallShipList.UpdateEntry(Entity);
				break;
			case MyDetectedEntityType.LargeGrid:
				LargeShipList.UpdateEntry(Entity);
				break;
			case MyDetectedEntityType.CharacterHuman:
				CharacterList.UpdateEntry(Entity);
				break;
			case MyDetectedEntityType.CharacterOther:
				CharacterList.UpdateEntry(Entity);
				break;
		}
	}
	
	for(int i=0;i<SmallShipList.Count;i++){
		bool Remove=false;
		EntityInfo Entity=SmallShipList[i];
		foreach(IMySensorBlock Sensor in AllSensors){
			if(CanDetect(Sensor,Entity)){
				if(IsDetecting(Sensor,Entity)){
					Remove=false;
					break;
				}
				else
					Remove=true;
			}
		}
		if(Remove){
			SmallShipList.RemoveEntry(Entity);
			i--;
		}
	}
	for(int i=0;i<LargeShipList.Count;i++){
		bool Remove=false;
		EntityInfo Entity=LargeShipList[i];
		foreach(IMySensorBlock Sensor in AllSensors){
			if(CanDetect(Sensor,Entity)){
				if(IsDetecting(Sensor,Entity)){
					Remove=false;
					break;
				}
				else
					Remove=true;
			}
		}
		if(Remove){
			LargeShipList.RemoveEntry(Entity);
			i--;
		}
	}
	for(int i=0;i<CharacterList.Count;i++){
		bool Remove=false;
		EntityInfo Entity=CharacterList[i];
		foreach(IMySensorBlock Sensor in AllSensors){
			if(CanDetect(Sensor,Entity)){
				if(IsDetecting(Sensor,Entity)){
					Remove=false;
					break;
				}
				else
					Remove=true;
			}
		}
		if(Remove){
			CharacterList.RemoveEntry(Entity);
			i--;
		}
	}
	
	PerformAlarm();
	
	for(int i=0; i<Airlocks.Count; i++){
		AirlockString += "Airlock "+(i+1).ToString()+" Status:\n";
		UpdateAirlock(Airlocks[i]);
	}
	
	foreach(IMyDoor AutoDoor in AutoDoors){
		bool found_entity=false;
		double min_distance_check=3.75*(1+(Controller.GetShipSpeed() / 200));
		foreach(EntityInfo Entity in CharacterList){
			if(Entity.Relationship != MyRelationsBetweenPlayerAndBlock.Enemies && Entity.Relationship != MyRelationsBetweenPlayerAndBlock.Neutral){
				Vector3D position=Entity.Position+CurrentVelocity/100;
				double distance=(AutoDoor.GetPosition()-Entity.Position).Length();
				bool is_closest_to_this_airlock=distance <= min_distance_check;
				if(distance<min_distance_check){
					found_entity=true;
					if(!_Lockdown)
						AutoDoor.OpenDoor();
					break;
				}
			}
		}
		if(!found_entity)
			AutoDoor.CloseDoor();
	}
	
	if(Command_Menu.AutoRefresh())
		DisplayMenu();
	
	Scan_Time=0;
	return true;
}

public void PerformDisarm(){
	List<IMyWarhead> Warheads=new List<IMyWarhead>();
	GridTerminalSystem.GetBlocksOfType<IMyWarhead>(Warheads);
	foreach(IMyWarhead Warhead in Warheads){
		if(HasBlockData(Warhead, "VerifiedWarhead") && GetBlockData(Warhead, "VerifiedWahead").Equals("Active"))
			continue;
		Warhead.DetonationTime=Math.Max(60, Warhead.DetonationTime);
		Warhead.IsArmed=false;
		Warhead.StopCountdown();
	}
}

void UpdateProgramInfo(){
	cycle_long+=((++cycle)/long.MaxValue)%long.MaxValue;
	cycle=cycle % long.MaxValue;
	switch(loading_char){
		case '|':
			loading_char='\\';
			break;
		case '\\':
			loading_char='-';
			break;
		case '-':
			loading_char='/';
			break;
		case '/':
			loading_char='|';
			break;
	}
	Echo(Program_Name+" OS "+cycle_long.ToString()+'-'+cycle.ToString()+" ("+loading_char+")");
	Me.GetSurface(1).WriteText(Program_Name+" OS "+cycle_long.ToString()+'-'+cycle.ToString()+" ("+loading_char+")", false);
	Me.GetSurface(0).WriteText("", false);
	seconds_since_last_update=Runtime.TimeSinceLastRun.TotalSeconds+(Runtime.LastRunTimeMs / 1000);
	if(seconds_since_last_update<1){
		Echo(Math.Round(seconds_since_last_update*1000, 0).ToString()+" milliseconds\n");
	}
	else if(seconds_since_last_update<60){
		Echo(Math.Round(seconds_since_last_update, 3).ToString()+" seconds\n");
	}
	else if(seconds_since_last_update/60<60){
		Echo(Math.Round(seconds_since_last_update/60, 2).ToString()+" minutes\n");
	}
	else if(seconds_since_last_update/60/60<24){
		Echo(Math.Round(seconds_since_last_update/60/60, 2).ToString()+" hours\n");
	}
	else {
		Echo(Math.Round(seconds_since_last_update/60/60/24, 2).ToString()+" days\n");
	}
}

double GetAngle(Vector3D v1, Vector3D v2){
	return GenericMethods<IMyTerminalBlock>.GetAngle(v1,v2);
}

//Sets gyroscope outputs from player input, dampeners, gravity, and autopilot
double Pitch_Time= 1.0f;
double Yaw_Time=1.0f;
double Roll_Time=1.0f;
void SetGyroscopes(){
	Gyroscope.GyroOverride=(AngularVelocity.Length()<3);
	float current_pitch=(float) Relative_AngularVelocity.X;
	float current_yaw=(float) Relative_AngularVelocity.Y;
	float current_roll=(float) Relative_AngularVelocity.Z;
	
	float gyro_count = 0;
	List<IMyGyro> AllGyros=new List<IMyGyro>();
	GridTerminalSystem.GetBlocksOfType<IMyGyro>(AllGyros);
	foreach(IMyGyro Gyro in AllGyros){
		if(Gyro.IsWorking)
			gyro_count+=Gyro.GyroPower/100.0f;
	}
	float gyro_multx=(float)Math.Max(0.1f, Math.Min(1, 1.5f/(Controller.CalculateShipMass().PhysicalMass/gyro_count/1000000)));
	
	float input_pitch=0;
	float input_yaw=0;
	float input_roll=0;
	
	if(Pitch_Time<1)
		Pitch_Time+=seconds_since_last_update;
	if(Yaw_Time<1)
		Yaw_Time+=seconds_since_last_update;
	if(Roll_Time<1)
		Roll_Time+=seconds_since_last_update;
	
	input_pitch=Math.Min(Math.Max(Controller.RotationIndicator.X / 100, -1), 1);
	if(Math.Abs(input_pitch)<0.05f){
		input_pitch=current_pitch*0.99f;
		if((((Elevation-MySize)<Controller.GetShipSpeed()*2&&(Elevation-MySize)<50)||(Controller.DampenersOverride&&!Controller.IsUnderControl))&&GetAngle(Gravity,Forward_Vector)<90&&Pitch_Time>=1){
			double difference=Math.Abs(GetAngle(Gravity,Forward_Vector));
			if(difference<90)
				input_pitch-=10*gyro_multx*((float)Math.Min(Math.Abs((90-difference)/90), 1));
		}
		if((Controller.DampenersOverride&&!Controller.IsUnderControl)&&(GetAngle(Gravity,Forward_Vector)>(90+ACCEPTABLE_ANGLE/2))){
			double difference=Math.Abs(GetAngle(Gravity,Forward_Vector));
			if(difference>90+ACCEPTABLE_ANGLE/2)
				input_pitch+=10*gyro_multx*((float)Math.Min(Math.Abs((difference-90)/90), 1));
		}
		if(Match_Direction){
			double difference=GetAngle(Down_Vector, Target_Direction)-GetAngle(Up_Vector, Target_Direction);
			if(Math.Abs(difference)>1)
				input_pitch-=(float)Math.Min(Math.Max(difference/5,-4),4)*gyro_multx;
		}
	}
	else{
		Pitch_Time=0;
		input_pitch*=30;
	}
	input_yaw=Math.Min(Math.Max(Controller.RotationIndicator.Y / 100, -1), 1);
	if(Math.Abs(input_yaw)<0.05f){
		input_yaw=current_yaw*0.99f;
		if(Match_Direction){
			double difference=GetAngle(Left_Vector, Target_Direction)-GetAngle(Right_Vector, Target_Direction);
			if(Math.Abs(difference) > 1){
				input_yaw+=(float)Math.Min(Math.Max(difference/5,-4),4)*gyro_multx;
			}
		}
	}
	else{
		Yaw_Time=0;
		input_yaw*=30;
	}
	input_roll=Controller.RollIndicator;
	if(Math.Abs(input_roll)<0.05f){
		input_roll=current_roll*0.99f;
		if(Gravity.Length() > 0  && Roll_Time >= 1){
			double difference=GetAngle(Left_Vector, Gravity)-GetAngle(Right_Vector, Gravity);
			if(Math.Abs(difference)>ACCEPTABLE_ANGLE){
				input_roll-=(float)Math.Min(Math.Max(difference/5,-1),1)*gyro_multx;
			}
		}
	}
	else{
		Roll_Time=0;
		input_roll*=10;
	}
	
	Vector3D input=new Vector3D(input_pitch,input_yaw,input_roll);
	Vector3D global=Vector3D.TransformNormal(input,Controller.WorldMatrix);
	Vector3D output=Vector3D.TransformNormal(global,MatrixD.Invert(Gyroscope.WorldMatrix));
	output.Normalize();
	output*=input.Length();
	
	Gyroscope.Pitch=(float)output.X;
	Gyroscope.Yaw=(float)output.Y;
	Gyroscope.Roll=(float)output.Z;
}

//Sets thruster outputs from player input, dampeners, gravity, and autopilot
void SetThrusters(){
	float input_forward=0.0f;
	float input_up=0.0f;
	float input_right=0.0f;
	
	float damp_multx=0.99f;
	double effective_speed_limit=SPEED_LIMIT;
	
	if(Elevation<50)
		effective_speed_limit=Math.Min(effective_speed_limit, Elevation*2);
	if(Time_To_Crash<60 && Time_To_Crash>=0)
		effective_speed_limit=Math.Min(effective_speed_limit,Time_To_Crash/60*100);
	if(Controller.DampenersOverride){
		Write("Cruise Control: Off");
		input_right-=(float)((Relative_CurrentVelocity.X-Relative_RestingVelocity.X)*Mass_Accomodation*damp_multx);
		input_up-=(float)((Relative_CurrentVelocity.Y-Relative_RestingVelocity.Y)*Mass_Accomodation*damp_multx);
		input_forward+=(float)((Relative_CurrentVelocity.Z-Relative_RestingVelocity.Z)*Mass_Accomodation*damp_multx);
	}
	else {
		Write("Cruise Control: On");
		Vector3D velocity_direction=CurrentVelocity;
		velocity_direction.Normalize();
		double angle=Math.Min(GetAngle(Forward_Vector, velocity_direction), GetAngle(Backward_Vector, velocity_direction));
		if(angle <= ACCEPTABLE_ANGLE / 2){
			input_right -= (float) ((Relative_CurrentVelocity.X-Relative_RestingVelocity.X)*Mass_Accomodation*damp_multx);
			input_up -= (float) ((Relative_CurrentVelocity.Y-Relative_RestingVelocity.Y)*Mass_Accomodation*damp_multx);
			Write("Stabilizers: On ("+Math.Round(angle, 1)+"° dev)");
		}
		else {
			Write("Stabilizers: Off ("+Math.Round(angle, 1)+"° dev)");
		}
	}
	
	if(Target_Distance<Math.Max(1, Math.Min(10, MySize/10))){
		Match_Direction=false;
		Match_Position=false;
	}
	else {
		effective_speed_limit=Math.Min(effective_speed_limit, Math.Sqrt(Target_Distance/4)*4);
	}
	effective_speed_limit=Math.Max(effective_speed_limit, 5);
	
	if(Gravity.Length()>0&&Mass_Accomodation>0&&(Controller.GetShipSpeed()<100||GetAngle(CurrentVelocity,Gravity)>ACCEPTABLE_ANGLE)){
		if(!(_Autoland&&Time_To_Crash>15&&Controller.GetShipSpeed()>5)){
			input_right -= (float) Adjusted_Gravity.X;
			input_up -= (float) Adjusted_Gravity.Y;
			input_forward += (float) Adjusted_Gravity.Z;
		}
	}
	
	bool matched_direction=!Match_Direction;
	if(Match_Direction){
		matched_direction=Math.Abs(GetAngle(Target_Direction,Forward_Vector))<=ACCEPTABLE_ANGLE;
	}
	
	if(Math.Abs(Controller.MoveIndicator.X)>0.5f){
		if(Controller.MoveIndicator.X > 0){
			if((CurrentVelocity+Right_Vector-RestingVelocity).Length() <= effective_speed_limit)
				input_right=0.95f*Right_Thrust;
			else
				input_right=Math.Min(input_right, 0);
		} else {
			if((CurrentVelocity+Left_Vector-RestingVelocity).Length() <= effective_speed_limit)
				input_right=-0.95f*Left_Thrust;
			else
				input_right=Math.Max(input_right, 0);
		}
	}
	else if(Match_Position){
		double Relative_Speed=Relative_CurrentVelocity.X;
		double Relative_Target_Speed=RestingVelocity.X;
		double Relative_Distance=Relative_Target_Position.X;
		double deacceleration=0;
		double difference=Relative_Speed-Relative_Target_Speed;
		if(difference > 0){
			deacceleration=Math.Abs(difference) / Left_Thrust;
		}
		else if(difference<0){
			deacceleration=Math.Abs(difference) / Right_Thrust;
		}
		if((difference > 0) ^ (Relative_Distance<0)){
			double time=difference / deacceleration;
			time=(Relative_Distance-(difference*time/2))/difference;
			if(time > 0 && (!Match_Direction || matched_direction) && Relative_Speed-Relative_Target_Speed<=0.05){
				if(difference > 0){
					if((CurrentVelocity+Left_Vector-RestingVelocity).Length() <= Math.Min(Elevation, Math.Min(effective_speed_limit, Target_Distance)))
						input_right=-0.95f*Left_Thrust;
				}
				else {
					if((CurrentVelocity+Right_Vector-RestingVelocity).Length() <= Math.Min(Elevation, Math.Min(effective_speed_limit, Target_Distance)))
						input_right=0.95f*Right_Thrust;
				}
			}
		}
	}
	
	if(Math.Abs(Controller.MoveIndicator.Y)>0.5f){
		if(Controller.MoveIndicator.Y > 0){
			if((CurrentVelocity+Up_Vector-RestingVelocity).Length() <= effective_speed_limit)
				input_up=0.95f*Up_Thrust;
			else
				input_up=Math.Min(input_up, 0);
		} else {
			if((CurrentVelocity+Down_Vector-RestingVelocity).Length() <= effective_speed_limit)
				input_up=-0.95f*Down_Thrust;
			else
				input_up=Math.Max(input_up, 0);
		}
	}
	else if(Match_Position){
		double Relative_Speed=Relative_CurrentVelocity.Y;
		double Relative_Target_Speed=RestingVelocity.Y;
		double Relative_Distance=Relative_Target_Position.Y;
		double deacceleration=0;
		double difference=Relative_Speed-Relative_Target_Speed;
		if(difference > 0){
			deacceleration=Math.Abs(difference) / Down_Thrust;
		}
		else if(difference<0){
			deacceleration=Math.Abs(difference) / Up_Thrust;
		}
		if((difference > 0) ^ (Relative_Distance<0)){
			double time=difference / deacceleration;
			time=(Relative_Distance-(difference*time/2))/difference;
			if(time > 0 && (!Match_Direction || matched_direction) && Relative_Speed-Relative_Target_Speed<=0.05){
				if(difference > 0){
					if((CurrentVelocity+Down_Vector-RestingVelocity).Length() <= Math.Min(Elevation, Math.Min(effective_speed_limit, Target_Distance)))
						input_up=-0.95f*Down_Thrust;
				}
				else {
					if((CurrentVelocity+Up_Vector-RestingVelocity).Length() <= Math.Min(Elevation, Math.Min(effective_speed_limit, Target_Distance)))
						input_up=0.95f*Up_Thrust;
				}
			}
		}
	}
	
	if(Math.Abs(Controller.MoveIndicator.Z)>0.5f){
		if(Controller.MoveIndicator.Z<0){
			if((CurrentVelocity+Up_Vector-RestingVelocity).Length() <= effective_speed_limit)
				input_forward=0.95f*Forward_Thrust;
			else
				input_forward=Math.Min(input_forward, 0);
		} else {
			if((CurrentVelocity+Down_Vector-RestingVelocity).Length() <= effective_speed_limit)
				input_forward=-0.95f*Backward_Thrust;
			else
				input_forward=Math.Max(input_forward, 0);
		}
	}
	else if(Match_Position){
		double Relative_Speed=Relative_CurrentVelocity.Z;
		double Relative_Target_Speed=RestingVelocity.Z;
		double Relative_Distance=Relative_Target_Position.Z;
		double deacceleration=0;
		double difference=Relative_Speed-Relative_Target_Speed;
		if(difference > 0){
			deacceleration=Math.Abs(difference) / Backward_Thrust;
		}
		else if(difference<0){
			deacceleration=Math.Abs(difference) / Forward_Thrust;
		}
		if((difference > 0) ^ (Relative_Distance<0)){
			double time=difference / deacceleration;
			time=(Relative_Distance-(difference*time/2))/difference;
			if(time > 0 && (!Match_Direction || matched_direction) && Relative_Speed-Relative_Target_Speed<=0.05){
				if(difference > 0){
					if((CurrentVelocity+Down_Vector-RestingVelocity).Length() <= Math.Min(Elevation, Math.Min(effective_speed_limit, Target_Distance)))
						input_forward=-0.95f*Backward_Thrust;
				}
				else {
					if((CurrentVelocity+Up_Vector-RestingVelocity).Length() <= Math.Min(Elevation, Math.Min(effective_speed_limit, Target_Distance)))
						input_forward=0.95f*Forward_Thrust;
				}
			}
		}
	}
	
	float output_forward=0.0f;
	float output_backward=0.0f;
	if(input_forward / Forward_Thrust > 0.05f)
		output_forward=Math.Min(Math.Abs(input_forward / Forward_Thrust), 1);
	else if(input_forward / Backward_Thrust<-0.05f)
		output_backward=Math.Min(Math.Abs(input_forward / Backward_Thrust), 1);
	float output_up=0.0f;
	float output_down=0.0f;
	if(input_up / Up_Thrust > 0.05f)
		output_up=Math.Min(Math.Abs(input_up / Up_Thrust), 1);
	else if(input_up / Down_Thrust<-0.05f)
		output_down=Math.Min(Math.Abs(input_up / Down_Thrust), 1);
	float output_right=0.0f;
	float output_left=0.0f;
	if(input_right / Right_Thrust > 0.05f)
		output_right=Math.Min(Math.Abs(input_right / Right_Thrust), 1);
	else if(input_right / Left_Thrust<-0.05f)
		output_left=Math.Min(Math.Abs(input_right / Left_Thrust), 1);
	
	foreach(IMyThrust Thruster in Forward_Thrusters){
		Thruster.ThrustOverridePercentage=output_forward;
	}
	foreach(IMyThrust Thruster in Backward_Thrusters){
		Thruster.ThrustOverridePercentage=output_backward;
	}
	foreach(IMyThrust Thruster in Up_Thrusters){
		Thruster.ThrustOverridePercentage=output_up;
	}
	foreach(IMyThrust Thruster in Down_Thrusters){
		Thruster.ThrustOverridePercentage=output_down;
	}
	foreach(IMyThrust Thruster in Right_Thrusters){
		Thruster.ThrustOverridePercentage=output_right;
	}
	foreach(IMyThrust Thruster in Left_Thrusters){
		Thruster.ThrustOverridePercentage=output_left;
	}
}

//Sets directional vectors, Elevation, etc
void GetPositionData(){
	Write("", false, false);
	Vector3D base_vector=new Vector3D(0,0,-1);
	Forward_Vector=LocalToGlobal(base_vector);
	Forward_Vector.Normalize();
	
	base_vector=new Vector3D(0,1,0);
	Up_Vector=LocalToGlobal(base_vector);
	Up_Vector.Normalize();
	
	base_vector=new Vector3D(-1,0,0);
	Left_Vector=LocalToGlobal(base_vector);
	Left_Vector.Normalize();
	
	Gravity=Controller.GetNaturalGravity();
	CurrentVelocity=Controller.GetShipVelocities().LinearVelocity;
	AngularVelocity=Controller.GetShipVelocities().AngularVelocity;
	
	Time_To_Crash=-1;
	Elevation=double.MaxValue;
	if(Controller.TryGetPlanetElevation(MyPlanetElevation.Sealevel, out Sealevel)){
		if(Controller.TryGetPlanetPosition(out PlanetCenter)){
			if(Sealevel<6000 && Controller.TryGetPlanetElevation(MyPlanetElevation.Surface, out Elevation)){
				if(Sealevel > 5000){
					double difference=Sealevel-5000;
					Elevation= ((Elevation*(1000-difference))+(Sealevel*difference)) / 1000;
				}
				else if(Elevation<50){
					double terrain_height=(Controller.GetPosition()-PlanetCenter).Length()-Elevation;
					List<IMyTerminalBlock> AllBlocks=new List<IMyTerminalBlock>();
					GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(AllBlocks);
					foreach(IMyTerminalBlock Block in AllBlocks){
						Elevation=Math.Min(Elevation, (Block.GetPosition()-PlanetCenter).Length()-terrain_height);
					}
				}
			}
			else {
				Elevation=Sealevel;
			}
			if(!Me.CubeGrid.IsStatic){
				double from_center=(Controller.GetPosition()-PlanetCenter).Length();
				Vector3D next_position=Controller.GetPosition()+1*CurrentVelocity;
				double Elevation_per_second=(from_center-(next_position-PlanetCenter).Length());
				Time_To_Crash=Elevation/Elevation_per_second;
				Vector3D Closest_Crash_Direction=Closest_Hit_Position;
				Closest_Crash_Direction.Normalize();
				Vector3D Movement_Direction=CurrentVelocity;
				Movement_Direction.Normalize();
				if(GetAngle(Movement_Direction,Closest_Crash_Direction)<=ACCEPTABLE_ANGLE){
					Time_To_Crash=Math.Min(Time_To_Crash,(Closest_Hit_Position-Controller.GetPosition()).Length()/CurrentVelocity.Length());
				}
				
				bool need_print=true;
				if(Time_To_Crash>0){
					if(Time_To_Crash<15 && Controller.GetShipSpeed() > 5){
						Controller.DampenersOverride=true;
						RestingVelocity=new Vector3D(0,0,0);
						Write("Crash predicted within 15 seconds; enabling Dampeners");
						need_print=false;
					}
					else if(Time_To_Crash*Math.Max(Elevation,1000)<1800000 && Controller.GetShipSpeed() > 1.0f){
						Write(Math.Round(Time_To_Crash, 1).ToString()+" seconds to crash");
						if(_Autoland && Time_To_Crash>30)
							Controller.DampenersOverride=false;
						need_print=false;
					}
					if(Elevation-MySize<5&&_Autoland)
						_Autoland=false;
				}
				if(need_print)
					Write("No crash likely at current velocity");
			}
		}
		else {
			PlanetCenter=new Vector3D(0,0,0);
		}
	}
	else{
		Sealevel=double.MaxValue;
	}
	if(Match_Position||Tracking){
		Target_Position+=seconds_since_last_update*RestingVelocity;
	}
	if(Match_Direction){
		Target_Direction=Target_Position-Controller.GetPosition();
		Target_Direction.Normalize();
	}
	
	Mass_Accomodation=(float)(Controller.CalculateShipMass().PhysicalMass*Gravity.Length());
	
}

public void Main(string argument, UpdateType updateSource)
{
	try{
		UpdateProgramInfo();
		GetPositionData();
		if(!Me.CubeGrid.IsStatic)
			Write("Elevation: "+Math.Round(Elevation,1).ToString());
		Scan_Time+=seconds_since_last_update;
		if(Scan_Time >= Scan_Frequency){
			PerformScan();
		}
		else
			Write("Last Scan "+Math.Round(Scan_Time,1).ToString());
		Write(ScanString);
		Echo(AirlockString);
		
		if(Match_Direction){
			double angle=GetAngle(Target_Direction, Forward_Vector);
			Write("Angle Difference: "+Math.Round(angle,1).ToString()+"°");
		}
		if(Match_Position){
			double distance=(Target_Position-Controller.GetPosition()).Length()-MySize/2;
			string distance_string=Math.Round(distance,0).ToString()+"M";
			if(distance>=1000){
				distance_string=Math.Round(distance/1000,1).ToString()+"kM";
			}
			Write("Distance: "+distance_string);
		}
		
		if(argument.ToLower().Equals("back")){
			Command_Menu.Back();
			DisplayMenu();
		}
		else if(argument.ToLower().Equals("prev")){
			Command_Menu.Prev();
			DisplayMenu();
		}
		else if(argument.ToLower().Equals("next")){
			Command_Menu.Next();
			DisplayMenu();
		}
		else if(argument.ToLower().Equals("select")){
			Command_Menu.Select();
			DisplayMenu();
		}
		else if(argument.ToLower().Equals("lockdown")){
			Lockdown();
		}
		else if(argument.ToLower().Equals("autoland")){
			Autoland();
		}
		else if(argument.ToLower().Equals("factory reset")){
			FactoryReset();
			DisplayMenu();
		}
		if(_Autoland)
			Write("Autoland Enabled");
		
		if(Tracking){
			double distance=(Target_Position-Controller.GetPosition()).Length();
			string distance_string=Math.Round(distance,0).ToString()+"M";
			if(distance>=1000)
				distance_string=Math.Round(distance/1000,1).ToString()+"kM";
			Write("Tracking target at "+distance_string);
		}
		
		Echo((new GenericMethods<IMyDoor>(this)).GetAllIncluding("Air Seal").Count.ToString()+" Air Seals");
		
		if(!Me.CubeGrid.IsStatic && Controller.CalculateShipMass().PhysicalMass>0){
			SetThrusters();
			SetGyroscopes();
		}
		
		switch(ShipStatus){
			case AlertStatus.Green:
				SetStatus("Condition "+ShipStatus.ToString()+Submessage, new Color(137, 255, 137, 255), new Color(0, 151, 0, 255));
				break;
			case AlertStatus.Blue:
				SetStatus("Condition "+ShipStatus.ToString()+Submessage, new Color(137, 239, 255, 255), new Color(0, 88, 151, 255));
				break;
			case AlertStatus.Yellow:
				SetStatus("Condition "+ShipStatus.ToString()+Submessage, new Color(255, 239, 137, 255), new Color(66, 66, 0, 255));
				break;
			case AlertStatus.Orange:
				SetStatus("Condition "+ShipStatus.ToString()+Submessage, new Color(255, 197, 0, 255), new Color(88, 44, 0, 255));
				break;
			case AlertStatus.Red:
				SetStatus("Condition "+ShipStatus.ToString()+Submessage, new Color(255, 137, 137, 255), new Color(151, 0, 0, 255));
				break;
		}
		
		foreach(IMyCameraBlock Camera in GetValidCameras()){
			Write(Camera.CustomName+" Charge: "+Math.Round(Camera.AvailableScanRange/1000,1).ToString()+"kM");
		}
		
		Runtime.UpdateFrequency=GetUpdateFrequency();
	}
	catch(Exception E){
		Write(E.ToString());
		FactoryReset();
		DisplayMenu();
	}
}
