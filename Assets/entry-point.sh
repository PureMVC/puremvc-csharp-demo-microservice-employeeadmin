#!/usr/bin/env bash

#wait for the SQL Server to come up
sleep 15s

#run the setup script to create the DB and the schema in the DB
/opt/mssql-tools/bin/sqlcmd -S ${SA_HOST} -U sa -P ${SA_PASSWORD} -d master -i Schema.sql