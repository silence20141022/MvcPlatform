﻿function panel_file_ini() {
    var store_filetype1 = Ext.create('Ext.data.JsonStore', {
        fields: ['FILETYPEID', 'FILETYPENAME'],
        data: [{ FILETYPEID: '44', FILETYPENAME: '订单文件' },           
            { FILETYPEID: '58', FILETYPENAME: '配舱单文件' }] 
    })
    var combo_filetype1 = Ext.create('Ext.form.field.ComboBox', {//文件类型
        name: 'FILETYPEID',
        store: store_filetype1,
        fieldLabel: '文件类型',
        displayField: 'FILETYPENAME',
        valueField: 'FILETYPEID',
        queryMode: 'local',
        labelWidth: 60,
        labelAlign: 'right',
        width: 160,
        value: '44',
        editable: false
    })
    var store_ietype1 = Ext.create('Ext.data.JsonStore', {
        fields: ['CODE', 'NAME'],
        data: ietype_js_data
    })
    var combo_ietype1 = Ext.create('Ext.form.field.ComboBox', {
        id: 'combo_ietype1',
        store: store_ietype1,
        labelAlign: 'right',
        fieldLabel: '进出口类型',
        displayField: 'NAME',
        valueField: 'CODE',
        queryMode: 'local',
        editable: false,
        labelWidth: 70,
        width: 170,
        value: '进/出口业务'
    })
    var bbar_r = '<div class="btn-group">'
        + '<button type="button" onclick="order_cancel_submit()" class="btn btn-primary btn-sm" id="btn_cancelsubmit"><i class="fa fa-angle-double-left"></i>&nbsp;撤单</button>'
        + '<button type="button" onclick="addGlyw()" class="btn btn-primary btn-sm" id="btn_addlinkorder"><i class="fa fa-plus fa-fw"></i>&nbsp;新增关联业务</button>'
        + '<button type="button" onclick="clearWin()" class="btn btn-primary btn-sm" id="btn_createorder"><i class="fa fa-plus fa-fw"></i>&nbsp;新增</button>'
        //+ '<button type="button" onclick="LoadOrderFzxz()" class="btn btn-primary btn-sm"><i class="fa fa-files-o"></i>&nbsp;复制新增</button>'
        + '<button type="button" onclick="save()" class="btn btn-primary btn-sm"><i class="fa fa-floppy-o"></i>&nbsp;保存</button>'
        + '<button type="button" onclick="submit()" class="btn btn-primary btn-sm" id="btn_submitorder"><i class="fa fa-hand-o-up"></i>&nbsp;提交委托</button>'
        + '</div>';
    var bbar_l = '<div class="btn-group">'
                + '<button type="button" class="btn btn-primary btn-sm" id="pickfiles"><i class="fa fa-upload"></i>&nbsp;上传文件</button>'
                + '<button type="button" onclick="browsefile()" class="btn btn-primary btn-sm"><i class="fa fa-exchange fa-fw"></i>&nbsp;浏览文件</button>'
                + '<button type="button" onclick="removeFile()" class="btn btn-primary btn-sm" id="deletefile"><i class="fa fa-trash-o"></i>&nbsp;删除文件</button>'
            + '</div>';
    toolbar1 = Ext.create('Ext.toolbar.Toolbar', {
        items: [combo_filetype1, combo_ietype1, bbar_l, '->', bbar_r]
    })
    store_file1 = Ext.create('Ext.data.JsonStore', {
        fields: ['ID', 'FILENAME', 'ORIGINALNAME', 'FILETYPE', 'FILETYPENAME', 'UPLOADTIME', 'SIZES', 'IETYPE'],
        listeners: {
            datachanged: function (sf_1, eOpts) {
                if (sf_1.find('IETYPE', '仅进口') >= 0 && Ext.getCmp('field_status1').getValue() == 1) {
                    Ext.getCmp('field_status1').setValue(10);
                }
                if (sf_1.find('IETYPE', '仅进口') < 0 && Ext.getCmp('field_status1').getValue() == 10) {
                    Ext.getCmp('field_status1').setValue(1);
                }
                if (sf_1.find('IETYPE', '仅出口') >= 0 && Ext.getCmp('field_status2').getValue() == 1) {
                    Ext.getCmp('field_status2').setValue(10);
                }
                if (sf_1.find('IETYPE', '仅出口') < 0 && Ext.getCmp('field_status2').getValue() == 10) {
                    Ext.getCmp('field_status2').setValue(1);
                }
            }
        }
    })
    var tmp1 = new Ext.XTemplate(
        '<tpl for=".">',
        '<div class="panel panel-default thumb-wrap fl" style="margin-top:5px;margin-left:5px;width:240px">',
        '<div class="panel-heading" style="padding-left:5px;padding-right:5px">{[values.ORIGINALNAME.substr(0,23)]}<div class="fr"><span class="glyphicon glyphicon-paperclip"></span></div></div>',
        '<div class="panel-body" style="padding-left:5px;">{FILETYPENAME}|',
        '<tpl>{[values.SIZES/1024 > 1024?Math.round(values.SIZES/(1024*1024))+"M":Math.round(values.SIZES/1024)+"K"]}</tpl>',
        '|{IETYPE}|{[values.UPLOADTIME.substr(0,values.UPLOADTIME.indexOf("T"))]}</div></div>',
        '</tpl>'
        )
    var fileview1 = Ext.create('Ext.view.View', {
        id: 'fileview1',
        store: store_file1,
        tpl: tmp1,
        itemSelector: 'div.thumb-wrap',
        multiSelect: true
    })
    panel_file1 = Ext.create('Ext.panel.Panel', {
        title: '<h4 style="margin-top:2px;margin-bottom:2px"><span class="label label-default"><i class="fa fa-chevron-circle-down"></i>&nbsp;随附文件</span></h4>',
        border: 0,
        width: '62%',
        minHeight: 100,
        items: [fileview1]
    })
}