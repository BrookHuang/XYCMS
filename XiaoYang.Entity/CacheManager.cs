﻿using System;
using System.Collections.Generic;
using System.Text;

namespace XiaoYang.Entity {
    public class CacheManager {

        static Dictionary<long, Cache> _collection;
        static CacheManager() {
            _collection = new Dictionary<long, Cache>();
            Init();
        }

        public static Cache Get(long inTypeID){
            if (_collection.ContainsKey(inTypeID)) {
                return _collection[inTypeID];
            }
            throw new Exception("Invalid type ID.");
        }

        public static void Clear() {
            _collection = new Dictionary<long, Cache>();
        }

        private static void Init() {
            System.Data.DataTable _table = EntityType.GetAll();
            for (int i = 0; i < _table.Rows.Count; i++) {
                EntityType _temp = new EntityType();
                _temp.Fill(_table.Rows[i]);
                _collection.Add(_temp.ID, new Cache(_temp.ID));
            }
        }
    }

    public class Cache {

        private System.Collections.ObjectModel.ReadOnlyCollection<EntityField> _fieldList;
        public System.Collections.ObjectModel.ReadOnlyCollection<EntityField> FieldList { get { return _fieldList; } }

        private System.Collections.ObjectModel.ReadOnlyCollection<EntityTable> _tableList;
        public System.Collections.ObjectModel.ReadOnlyCollection<EntityTable> TableList { get { return _tableList; } }

        private string[] _attributeNameList;
        public string[] AttributeNameList { get { return _attributeNameList; } }

        private EntityField _primaryField;
        public EntityField PrimaryField { get { return _primaryField; } }
        private EntityTable _mainTable;
        public EntityTable MainTable { get { return _mainTable; } }
        private EntityType _type;
        public EntityType Type { get { return _type; } }        

        internal Cache(long inTypeID) {
            List<EntityField> _tempFieldList = new List<EntityField>();
            List<EntityTable> _tempTableList = new List<EntityTable>();
            List<string> _tempAttributeName = new List<string>();
            _type = EntityType.GetInstance(inTypeID);
            System.Data.DataTable _tempTables = EntityTable.GetListByTypeID(_type.ID);
            int _fieldFirst = 0;
            for (int i = 0; i < _tempTables.Rows.Count; i++) {
                EntityTable _tempTable = new EntityTable();
                _tempTable.Fill(_tempTables.Rows[i]);
                if (_tempTable.Main) {
                    _mainTable = _tempTable;
                    _tempTableList.Insert(0, _tempTable);
                } else { 
                    _tempTableList.Add(_tempTable);
                }
                _tempTable.Key = string.Concat(EntityTable.TABLE_PREFIX, _tempTable.Key);
                System.Data.DataTable _tempFields = EntityField.GetListByTabldID(_tempTable.ID);
                for (int j = 0; j < _tempFields.Rows.Count; j++) {
                    EntityField _tempField = new EntityField();
                    _tempAttributeName.Add(_tempField.Key);
                    _tempField.Fill(_tempFields.Rows[j]);
                    _tempField.Table = _tempTable;
                    if (_tempField.Primary) {
                        _tempFieldList.Insert(_fieldFirst, _tempField);
                        if (_tempTable.Main) {
                            _primaryField = _tempField;
                        }
                    } else {
                        _tempFieldList.Add(_tempField);
                    }                    
                }
                _fieldFirst += _tempFields.Rows.Count;
            }
            _fieldList = new System.Collections.ObjectModel.ReadOnlyCollection<EntityField>(_tempFieldList);
            _tableList = new System.Collections.ObjectModel.ReadOnlyCollection<EntityTable>(_tempTableList);
            _attributeNameList = _tempAttributeName.ToArray();
        }
    }
}
