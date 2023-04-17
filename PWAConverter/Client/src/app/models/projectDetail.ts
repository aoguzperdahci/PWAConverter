import { SourceContainer } from "./sourceContainer";
import { SourceMap } from "./sourceMap";

export interface ProjectDetail {
  sourceMapList: SourceMap[];
  sourceContainers: SourceContainer[];

}
