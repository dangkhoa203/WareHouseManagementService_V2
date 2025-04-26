import {AgGridReact} from "ag-grid-react";
import { themeQuartz, iconSetQuartzBold,ColDef } from 'ag-grid-community';
import {useEffect, useState} from "react";
import { AG_GRID_LOCALE_VN } from '@ag-grid-community/locale';

export default function Test(){

    const myTheme = themeQuartz
        .withPart(iconSetQuartzBold)
        .withParams({
            accentColor: "#3CE4F754",
            backgroundColor: "#0B3745",
            borderColor: "#0FE2FA36",
            borderRadius: 0,
            cellHorizontalPaddingScale: 1.5,
            cellTextColor: "#9EC9E6",
            columnBorder: true,
            fontFamily: {
                googleFont: "IBM Plex Mono"
            },
            fontSize: 12,
            dataFontSize:14,
            foregroundColor: "#0EE2FA",
            headerBackgroundColor: "#ED6C02",
            headerFontFamily: {
                googleFont: "Roboto"
            },
            headerFontSize: 20,
            headerFontWeight: 800,
            headerRowBorder: true,
            headerTextColor: "#FDFDFD",
            headerVerticalPaddingScale: 1.2,
            iconSize: 24,
            iconColor: "#FDFDFD",
            iconButtonColor: "#FDFDFD",
            oddRowBackgroundColor: "#0B3745",
            rowBorder: true,
            rowVerticalPaddingScale: 1.2,
            spacing: 4,
            wrapperBorder: false,
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