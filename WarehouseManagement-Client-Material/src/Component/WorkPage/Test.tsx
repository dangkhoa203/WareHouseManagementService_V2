import {AgGridReact} from "ag-grid-react";
import { themeQuartz,ColDef } from 'ag-grid-community';
import {useEffect, useState} from "react";
import { AG_GRID_LOCALE_VN } from '@ag-grid-community/locale';

export default function Test(){

    const myTheme = themeQuartz
        .withParams({
            accentColor: "#00A2FF",
            backgroundColor: "#21222C",
            borderColor: "#C85A00",
            borderRadius: 0,
            browserColorScheme: "dark",
            cellHorizontalPaddingScale: 0.8,
            cellTextColor: "#00A2FF",
            checkboxUncheckedBackgroundColor: "#3F4156",
            columnBorder: true,
            dataFontSize: 15,
            fontFamily: {
                googleFont: "IBM Plex Mono"
            },
            fontSize: 13,
            foregroundColor: "#ED6C02",
            headerBackgroundColor: "#21222C",
            headerFontSize: 15,
            headerFontWeight: 700,
            headerTextColor: "#ED6C02",
            headerVerticalPaddingScale: 1.5,
            iconSize: 17,
            oddRowBackgroundColor: "#21222C",
            rangeSelectionBackgroundColor: "#FFFF0020",
            rangeSelectionBorderStyle: "solid",
            rowBorder: true,
            rowVerticalPaddingScale: 1.5,
            sidePanelBorder: true,
            spacing: 4,
            wrapperBorder: true,
            wrapperBorderRadius: 0
        });
    const [rowData, setRowData] = useState([]);
    const [colDefs, setColDefs] = useState<ColDef[]>([
        { field: "mission",filter:true,resizable:false,unSortIcon: true,flex: 2,minWidth:200,floatingFilter: true },
        { field: "company",resizable:false,flex: 1 },
        { field: "location",resizable:false,flex: 3 },
        { field: "date",resizable:false,flex: 1,filter:true },
        { field: "price",resizable:false,flex: 1,valueFormatter: (params:any) => { return '£ ' + params.value.toLocaleString(); } },
        { field: "successful",resizable:false,filter:true,flex: 1 },
        { field: "rocket",resizable:false,flex: 1 }
    ]);
    useEffect(() => {
        fetch('https://www.ag-grid.com/example-assets/space-mission-data.json') // Fetch data from server
            .then(result => result.json()) // Convert to JSON
            .then(rowData => setRowData(rowData)); // Update state of `rowData`
    }, [])
    return(
        <div style={{ height: "90%" }}>
            <AgGridReact
                rowData={rowData}
                columnDefs={colDefs}
                theme={myTheme}
                pagination={true}
                localeText={AG_GRID_LOCALE_VN}
            />
        </div>
    )
}