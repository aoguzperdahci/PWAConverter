import { AdditionalFeatures } from "./additionalFeatures";
import { ProjectOptions } from "./projectOptions";
import { SourceContainer } from "./sourceContainer";
import { SourceMap } from "./sourceMap";

export interface ProjectDetail {
  sourceMapList: SourceMap[];
  sourceContainers: SourceContainer[];
  additionalFeatures: AdditionalFeatures[];
  options: ProjectOptions;
}
