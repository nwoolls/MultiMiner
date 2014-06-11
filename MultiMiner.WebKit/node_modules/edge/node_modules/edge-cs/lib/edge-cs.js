exports.getCompiler = function () {
	return process.env.EDGE_CS_NATIVE || (__dirname + '\\edge-cs.dll');
};
