using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Commands;

namespace Buttons
{
    public class MainViewViewModel
    {
        private ExternalCommandData _commandData;

        public DelegateCommand SelectCommandPipes { get; }
        public DelegateCommand SelectCommandWalls { get; }
        public DelegateCommand SelectCommandDoors { get; }

        public MainViewViewModel(ExternalCommandData commandData)

        {
            _commandData = commandData;
            SelectCommandPipes = new DelegateCommand(OnSelectCommandPipes);
            SelectCommandWalls = new DelegateCommand(OnSelectCommandWalls);
            SelectCommandDoors = new DelegateCommand(OnSelectCommandDoors);
        }

        public event EventHandler HideRequest;
        private void RaiseHideRequest()
        {
            HideRequest?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler ShowRequest;
        private void RaiseShowRequest()
        {
            ShowRequest?.Invoke(this, EventArgs.Empty);
        }

        private void OnSelectCommandPipes()        // количество всех труб
        {
            RaiseHideRequest();

            UIApplication uiapp = _commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            var pipes = new FilteredElementCollector(doc)
                           .OfClass(typeof(Pipe))
                           .Cast<Pipe>()
                           .ToList();

            TaskDialog.Show("Сообщение", $"Количество труб = {pipes.Count.ToString()}");

            RaiseShowRequest();
        }

        private void OnSelectCommandWalls()     // объем всех стены
        {
            RaiseHideRequest();

            UIApplication uiapp = _commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            var walls = new FilteredElementCollector(doc)
                 .OfClass(typeof(Wall))
                 .ToElements();

            var volumeList = new List<double>();

            foreach (Wall element in walls)
            {

                Parameter volumeParameter = element.get_Parameter(BuiltInParameter.HOST_VOLUME_COMPUTED);

                if (volumeParameter.StorageType == StorageType.Double)
                {
                    double volume = UnitUtils.ConvertFromInternalUnits(volumeParameter.AsDouble(), UnitTypeId.CubicMeters);   // конвертация в метры
                    volumeList.Add(volume);
                }
            }

            TaskDialog.Show("Volume", $"Объем всех стен = {volumeList.Sum()} м3");

            RaiseShowRequest();
        }

        private void OnSelectCommandDoors()       //количество дверей
        {
            RaiseHideRequest();

            UIApplication uiapp = _commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            List<FamilyInstance> doors = new FilteredElementCollector(doc)
                           .OfCategory(BuiltInCategory.OST_Doors)
                           .WhereElementIsNotElementType()
                           .Cast<FamilyInstance>()
                           .ToList();

            TaskDialog.Show("Сообщение", $"Количество дверей = {doors.Count.ToString()}");

            RaiseShowRequest();
        }
    }
}
