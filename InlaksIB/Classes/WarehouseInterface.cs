using BackBone;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace InlaksIB
{
    public interface WarehouseInterface
    {

        List<ValuePair> getFilterColumns(string tablename);

        List<ValuePair> getViewColumns(string tablename);

        List<ValuePair> getDataSets(string moduleid);

        List<ValuePair> getTables();

        void DeleteDataSet(string datasetname);

        DataTable FilteredData(string dataset, List<DataSetFilter> filters);
    }
}