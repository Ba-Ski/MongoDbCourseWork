rs.initiate( {
   _id: "configReplSet",
   configsvr: true,
   members: [
      { _id: 0, host: "127.0.0.1:27000" },
      { _id: 1, host: "127.0.0.1:27001" }
   ]
} )