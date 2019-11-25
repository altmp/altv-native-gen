namespace Durty.AltV.NativesTypingsGenerator.Models.Typing
{
    /// <summary>
    /// type vectorPtr = Vector3;
    /// </summary>
    public class TypeDefType
    {
        public string Name { get; set; }

        public string TargetTypeName { get; set; }

        public override string ToString()
        {
            return $"type {Name} = {TargetTypeName};";
        }
    }
}
